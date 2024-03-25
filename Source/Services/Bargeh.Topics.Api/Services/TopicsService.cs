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
		// TODO: Should return a couple of posts too
		Topic topic =
			await dbContext.Topics.FirstOrDefaultAsync(t => t.ForumId.ToString() == request.Forum &&
															t.Permalink == request.Permalink)
			?? throw new RpcException(new(StatusCode.NotFound, "No topic was found with this permalink in this forum"));

		Post headPost = (await dbContext.Posts.FirstOrDefaultAsync(p => p.Topic == topic))!;

		return new()
		{
			Forum = topic.ForumId.ToString(),
			HeadPost = new()
			{
				Author = headPost.Author.ToString(),
				Body = headPost.Body,
			Likes = headPost.Likes,
			Loves = headPost.Loves,
			Funnies = headPost.Funnies,
			Insights = headPost.Insights,
			Dislikes = headPost.Dislikes
			},
			Title = topic.Title,
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
		// PRODUCTION: Validate image size and store it
		// PRODUCTION: It doesn't need topic id since it can get it from the parent post field

		if(string.IsNullOrWhiteSpace(request.Body))
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Whitespace parameters are not allowed"));
		}
		
		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		Post parent = await dbContext.Posts.Include(p => p.Topic)
									 .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.Parent))
					  ?? throw new RpcException(new(StatusCode.NotFound, "No parent post was found with this ID"));

		Post post = new()
		{
			Topic = parent.Topic,
			Author = userId,
			Body = request.Body,
			Parent = parent
		};

		await dbContext.AddAsync(post);

		// TODO: Set cancellation token for all like this
		await dbContext.SaveChangesAsync(context.CancellationToken);

		return new();
	}

	public override async Task<VoidOperationReply> ReactOnPost(ReactOnPostRequest request, ServerCallContext context)
	{
		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		Post post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id.ToString() == request.Post)
					?? throw new RpcException(new(StatusCode.NotFound, "No post was found with this ID"));

		Reaction? reaction =
			await dbContext.Reactions.FirstOrDefaultAsync(r => r.UserId == userId &&
															   r.Post.Id.ToString() == request.Post);

		// ReSharper disable once ConvertIfStatementToSwitchStatement
		if(request.State is ReactionUpdateState.None && reaction is not null)
		{
			dbContext.Remove(reaction);
			await dbContext.SaveChangesAsync();
			return new();
		}

		if(request.State is ReactionUpdateState.None && reaction is null)
		{
			return new();
		}

		// ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
		if(reaction is null)
		{
			reaction = new()
			{
				UserId = userId,
				Post = post,
				ReactionType = ReactionType.Like
			};
		}

		reaction.ReactionType = request.State switch
		{
			ReactionUpdateState.Love => ReactionType.Love,
			ReactionUpdateState.Funny => ReactionType.Funny,
			ReactionUpdateState.Insightful => ReactionType.Insightful,
			ReactionUpdateState.Dislike => ReactionType.Dislike,
			_ => reaction.ReactionType
		};

		dbContext.Update(reaction);
		await dbContext.SaveChangesAsync();

		return new();
	}

    public override Task<GetMorePostsReply> GetMorePosts(GetMorePostsRequest request, ServerCallContext context)
	{
		request.

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