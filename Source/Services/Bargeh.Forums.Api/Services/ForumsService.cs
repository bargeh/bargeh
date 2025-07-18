using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Forums.Api.Infrastructure.Models;
using Bargeh.Users.Api;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bargeh.Forums.Api.Services;

public class ForumsService(ForumsDbContext dbContext, UsersProto.UsersProtoClient usersService)
	: ForumsProto.ForumsProtoBase
{
	#region Static Methods

	private static async Task<IEnumerable<Claim>> ValidateAndGetUserClaims(string accessToken)
	{
		JwtSecurityTokenHandler tokenHandler = new();
		const string keyPath = "/Users/matin/sources/Bagreh/JwtPrivateKey.pfx";

		RsaSecurityKey securityKey = new(new X509Certificate2(keyPath, "bargeh.dev")
											 .GetRSAPrivateKey());


		TokenValidationResult tokenValidationResult =
			await tokenHandler.ValidateTokenAsync(accessToken, new()
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = securityKey,
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidIssuer = "https://bargeh.net",
				ValidAudience = "https://bargeh.net",
				ClockSkew = TimeSpan.Zero
			});

		if(!tokenValidationResult.IsValid)
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"AccessToken\" is not valid"));
		}

		IEnumerable<Claim> accessTokenClaims = tokenHandler.ReadJwtToken(accessToken).Claims!;
		return accessTokenClaims;
	}

	#endregion

	#region gRPC Endpoints

	public override async Task<AddForumReply> AddForum(AddForumRequest request, ServerCallContext context)
	{
		// TODO: Permalinks are accepted with spaces
		// TODO: Prevent duplicate forums
		// TODO: Add suffix to forums permalinks
		// TODO: Return forum's ID on success

		if(string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Description) ||
		   string.IsNullOrWhiteSpace(request.Permalink) || string.IsNullOrWhiteSpace(request.AccessToken))
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Whitespace parameters are not allowed"));
		}

		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		Forum forum = new()
		{
			Name = request.Name,
			Description = request.Description,
			Permalink = request.Permalink,
			OwnerId = userId,
			Members = 1
		};

		await dbContext.AddAsync(forum);
		await dbContext.SaveChangesAsync();

		await dbContext.ForumMemberships.AddAsync(new()
		{
			UserId = userId,
			Forum = forum
		});
		await dbContext.SaveChangesAsync();

		return new()
		{
			Permalink = forum.Permalink
		};

		// TODO: Add the owner as a member of the forum too
	}

	public override async Task<ProtoForum> GetForumByPermalink(GetForumByPermalinkRequest request,
															   ServerCallContext context)
	{
		Forum forum = await dbContext.Forums.FirstOrDefaultAsync(f => f.Permalink == request.Permalink)
					  ?? throw new RpcException(new(StatusCode.NotFound, "No forum was found with this permalink"));

		string ownerUsername = (await usersService.GetUserByIdAsync(new()
								   {
									   Id = forum.OwnerId.ToString()
								   })).Username;

		return new()
		{
			Id = forum.Id.ToString(),
			Name = forum.Name,
			Description = forum.Description,
			Members = forum.Members,
			OwnerUsername = ownerUsername,
			Supporters = forum.Supporters,
			Permalink = forum.Permalink,
			Avatar = forum.Avatar,
			Cover = forum.Cover
		};
	}

	public override async Task<ProtoForum> GetForumById(GetForumByIdRequest request, ServerCallContext context)
	{
		Forum forum = await dbContext.Forums.FirstOrDefaultAsync(f => f.Id == Guid.Parse(request.Id))
					  ?? throw new RpcException(new(StatusCode.NotFound, "No forum was found with this ID"));

		string ownerUsername = (await usersService.GetUserByIdAsync(new()
								   {
									   Id = forum.OwnerId.ToString()
								   })).Username;

		return new()
		{
			Id = forum.Id.ToString(),
			Name = forum.Name,
			Description = forum.Description,
			Members = forum.Members,
			OwnerUsername = ownerUsername,
			Supporters = forum.Supporters,
			Permalink = forum.Permalink,
			Avatar = forum.Avatar,
			Cover = forum.Cover
		};
	}

	public override async Task<Empty> JoinForum(JoinLeaveForumRequest request, ServerCallContext context)
	{
		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		Guid forumId = new(request.Forum);
		Forum forum = await dbContext.Forums.FirstOrDefaultAsync(f => f.Id == forumId)
					  ?? throw new RpcException(new(StatusCode.NotFound, "No forum with this permalink was found"));

		if(await dbContext.ForumMemberships.AnyAsync(m => m.UserId == userId && m.Forum == forum))
		{
			throw new RpcException(new(StatusCode.AlreadyExists, "This user is already a member of this forum"));
		}

		await dbContext.ForumMemberships.AddAsync(new()
		{
			UserId = userId,
			Forum = forum
		});

		forum.Members++;

		await dbContext.SaveChangesAsync();

		return new();
	}

	public override async Task<Empty> LeaveForum(JoinLeaveForumRequest request, ServerCallContext context)
	{
		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		Guid forumId = new(request.Forum);
		Forum forum = await dbContext.Forums.FirstOrDefaultAsync(f => f.Id == forumId)
					  ?? throw new RpcException(new(StatusCode.NotFound, "No forum with this permalink was found"));

		ForumMembership? forumMembership =
			await dbContext.ForumMemberships.FirstOrDefaultAsync(m => m.UserId == userId && m.Forum == forum);

		if(forumMembership is null)
		{
			throw new RpcException(new(StatusCode.AlreadyExists, "This user is not currently a member of this forum"));
		}

		if(userId == forumMembership.UserId)
		{
			throw new RpcException(new(StatusCode.PermissionDenied, "Forum owners can not leave their own forum"));
		}

		dbContext.Remove(forumMembership);

		forum.Members--;

		await dbContext.SaveChangesAsync();

		return new();
	}

	#endregion
}