using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bargeh.Topics.Api.Infrastructure;
using Bargeh.Topics.Api.Infrastructure.Models;
using Forums.Api;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Topics.Api;
using VoidOperationReply = Topics.Api.VoidOperationReply;

namespace Bargeh.Topics.Api.Services;

public class TopicsService(TopicsDbContext dbContext, ForumsProto.ForumsProtoClient forumsService)
	: TopicsProto.TopicsProtoBase
{
	public override async Task<TopicReply> GetTopicByPermalink(GetTopicByPermalinkRequest request,
															   ServerCallContext context)
	{
		Topic topic =
			await dbContext.Topics.FirstOrDefaultAsync(t => t.ForumId.ToString()
															 .Substring(0, t.ForumId.ToString().IndexOf('-')) ==
															request.Forum && t.Permalink == request.Permalink)
			?? throw new RpcException(new(StatusCode.NotFound, "No topic was found with this permalink"));

		Post headPost = (await dbContext.Posts.FirstOrDefaultAsync(p => p.Topic == topic))!;

		return new()
		{
			Forum = topic.ForumId.ToString(),
			Author = headPost.Author.ToString(),
			Title = topic.Title,
			Body = headPost.Body,
			Likes = headPost.Likes,
			Loves = headPost.Loves,
			Funnies = headPost.Funnies,
			Insights = headPost.Insights,
			Dislikes = headPost.Dislikes
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
			ForumId = Guid.Parse(request.Forum)
		};

		await dbContext.AddAsync(topic);
		await dbContext.SaveChangesAsync();

		Post firstPost = new()
		{
			Topic = topic,
			Author = userId,
			Body = request.Body
		};

		await dbContext.AddAsync(firstPost);
		await dbContext.SaveChangesAsync();

		return new()
		{
			Permalink = topic.Permalink
		};
	}

	public override async Task<VoidOperationReply> CreatePost(CreatePostRequest request, ServerCallContext context)
	{
		// PRODUCTION: Validate image size

		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		Topic topic = await dbContext.Topics.FirstOrDefaultAsync(t => t.Id == Guid.Parse(request.Topic))
					  ?? throw new RpcException(new(StatusCode.NotFound, "No topic was found with this ID"));
		throw new();
	}

	public override async Task<VoidOperationReply> ReactOnPost(ReactOnPostRequest request, ServerCallContext context)
	{
		throw new();
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