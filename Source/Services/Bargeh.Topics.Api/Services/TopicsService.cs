using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bargeh.Topics.Api.Infrastructure;
using Bargeh.Topics.Api.Infrastructure.Models;
using Forums.Api;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Topics.Api;

namespace Bargeh.Topics.Api.Services;

public class TopicsService(TopicsDbContext dbContext, ForumsProto.ForumsProtoClient forumsService)
	: TopicsProto.TopicsProtoBase
{
	public override async Task<TopicReply> GetTopicByPermalink(GetTopicByPermalinkRequest request,
															   ServerCallContext context)
	{
		Topic topic = await dbContext.Topics.FirstOrDefaultAsync(t => t.Permalink == request.Permalink)
					  ?? throw new RpcException(new(StatusCode.NotFound, "No topic was found with this permalink"));


		return new()
		{
			Forum = topic.ForumId.ToString(),
			Author = topic.AuthorId.ToString(),
			Title = topic.Title,
			Body = topic.Body,
			Likes = topic.Likes,
			Loves = topic.Loves,
			Funnies = topic.Funnies,
			Insights = topic.Insights,
			Dislikes = topic.Dislikes
		};
	}

	public override async Task<CreateTopicReply> CreateTopic(CreateTopicRequest request, ServerCallContext context)
	{
		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		try
		{
			await forumsService.GetForumByIdAsync(new()
			{
				Id = request.Forum
			});
		}
		catch(RpcException exception)
		{
			if(exception.StatusCode == StatusCode.NotFound)
			{
				throw new RpcException(exception.Status);
			}
		}

		if(string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Whitespace parameters are not allowed"));
		}

		Topic topic = new()
		{
			Title = request.Title,
			Body = request.Body,
			ForumId = Guid.Parse(request.Forum),
			AuthorId = userId
		};

		await dbContext.AddAsync(topic);
		await dbContext.SaveChangesAsync();

		return new()
		{
			Permalink = topic.Permalink
		};
	}
	
	

	#region Static Methods

	private static async Task<IEnumerable<Claim>> ValidateAndGetUserClaims(string accessToken)
	{
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