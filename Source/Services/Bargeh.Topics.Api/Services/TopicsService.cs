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
															   ServerCallContext callContext)
	{
		// TODO: Should return a couple of posts too
		Topic topic =
			await dbContext.Topics.FirstOrDefaultAsync(t => t.Forum.ToString() == request.Forum &&
															t.Permalink == request.Permalink)
			?? throw new RpcException(new(StatusCode.NotFound, "No topic was found with this permalink in this forum"));

		Post headPost = (await dbContext.Posts.FirstOrDefaultAsync(p => p.Topic == topic))!;

		return new()
		{
			Forum = topic.Forum.ToString(),
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

	public override async Task<CreateTopicReply> CreateTopic(CreateTopicRequest request, ServerCallContext callContext)
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
			Forum = Guid.Parse(request.Forum)
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

	public override async Task<VoidOperationReply> CreatePost(CreatePostRequest request, ServerCallContext callContext)
	{
		// PRODUCTION: Validate image size and store it
		// PRODUCTION: It doesn't need topic id since it can get it from the parent post field
		// TODO: What about top posts in postchains?

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
		await dbContext.SaveChangesAsync(callContext.CancellationToken);

		return new();
	}

	public override async Task<VoidOperationReply> ReactOnPost(ReactOnPostRequest request,
															   ServerCallContext callContext)
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

	public override async Task<GetMorePostChainsReply> GetMorePostChains(GetMorePostChainsRequest request,
																		 ServerCallContext callContext)
	{
		// TODO: Test the method
		await ValidateAndGetUserClaims(request.AccessToken);

		Post topicHeadPost =
			await dbContext.Posts.FirstOrDefaultAsync(p => p.Parent == null && p.Topic.ToString() == request.Topic) ??
			throw new RpcException(new(StatusCode.NotFound, "The topic with this ID was not found"));

		List<Post> newPostChains = await dbContext.Posts
												  .Where(p => !request.SeenPostchains.Contains(p.Id.ToString()) &&
															  p.Parent == topicHeadPost)
												  .Take(10)
												  .ToListAsync();


		List<ProtoPost> postsToReturn = [];
		foreach(Post headPost in newPostChains)
		{
			await AddPostHierarchyAsync(headPost.Id, postsToReturn);
		}

		GetMorePostChainsReply reply = new();
		reply.Posts.Add(postsToReturn);
		return reply;
	}

	private async Task AddPostHierarchyAsync(Guid parentId, ICollection<ProtoPost> posts)
	{
		Post? child = await dbContext.Posts.Include(p => p.Parent)
									 .FirstOrDefaultAsync(p => p.Parent != null && p.Parent.Id == parentId);
		if(child != null)
		{
			posts.Add(new()
			{
				Id = child.Id.ToString(),
				Body = child.Body,
				Likes = child.Likes,
				Loves = child.Loves,
				Insights = child.Insights,
				Funnies = child.Funnies,
				Dislikes = child.Dislikes,
				Author = child.Author.ToString(),
				Parent = child.Parent?.ToString(),
				Attachment = child.Attachment
			});

			await AddPostHierarchyAsync(child.Id, posts);
		}
	}


	#region Private Methods

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