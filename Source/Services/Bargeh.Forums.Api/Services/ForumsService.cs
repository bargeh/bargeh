using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Forums.Api.Infrastructure.Models;
using Forums.Api;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VoidOperationReply = Forums.Api.VoidOperationReply;

namespace Bargeh.Forums.Api.Services;

public class ForumsService(ForumsDbContext dbContext) : ForumsProto.ForumsProtoBase
{
	#region gRPC Endpoints

	public override async Task<VoidOperationReply> AddForum(AddForumRequest request, ServerCallContext context)
	{
		// PRODUCTION: Permalinks are accepted with spaces
		if(string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Description) ||
		   string.IsNullOrWhiteSpace(request.Permalink) || string.IsNullOrWhiteSpace(request.AccessToken))
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Whitespace parameters are not allowed"));
		}

		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);

		Forum forum = new()
		{
			Name = request.Name,
			Description = request.Description,
			Permalink = request.Permalink,
			OwnerId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value)
		};

		await dbContext.AddAsync(forum);
		await dbContext.SaveChangesAsync();

		return new();

		// TODO: Add the owner as a member of the forum too
	}

	public override async Task<ForumReply> GetForumByPermalink(GetForumByPermalinkRequest request,
															   ServerCallContext context)
	{
		Forum forum = await dbContext.Forums.FirstOrDefaultAsync(f => f.Permalink == request.Permalink)
					  ?? throw new RpcException(new(StatusCode.NotFound, "No forum was found with this permalink"));

		return new()
		{
			Name = forum.Name,
			Description = forum.Description,
			Members = forum.Members,
			Owner = forum.OwnerId.ToString(),
			Supporters = forum.Supporters
		};
	}

	public override async Task<VoidOperationReply> JoinForum(JoinForumRequest request, ServerCallContext context)
	{
		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		Forum forum = await dbContext.Forums.FirstOrDefaultAsync(f => f.Permalink == request.ForumPermalink)
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
		await dbContext.SaveChangesAsync();

		return new();
	}

	#endregion

	#region Static Methods

	private static async Task<IEnumerable<Claim>> ValidateAndGetUserClaims(string accessToken)
	{
		AddForumRequest request;
		JwtSecurityTokenHandler tokenHandler = new();
		SecurityKey key = new X509SecurityKey(new("C:/Source/Bargeh/JwtPublicKey.cer"));

		TokenValidationResult tokenValidationResult =
			await tokenHandler.ValidateTokenAsync(accessToken, new()
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = key,
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
}