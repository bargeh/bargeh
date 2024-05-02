using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Forums.Api.Infrastructure.Models;
using Forums.Api;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Users.Api;

namespace Bargeh.Forums.Api.Services;

public class TopicsService(
	ForumsDbContext dbContext,
	UsersProto.UsersProtoClient usersService)
	: TopicsProto.TopicsProtoBase
{
	private ForumsService _forumsService = new(dbContext, usersService);

	public override async Task<ProtoTopic> GetTopicByPermalink(GetTopicByPermalinkRequest request,
															   ServerCallContext callContext)
	{
		Topic topic =
			await dbContext.Topics.FirstOrDefaultAsync(t => t.Forum.ToString() == request.Forum &&
															t.Permalink == request.Permalink)
			?? throw new RpcException(new(StatusCode.NotFound, "No topic was found with this permalink in this forum"));

		Post headPost = (await dbContext.Posts.FirstOrDefaultAsync(p => p.Topic == topic && p.Parent == null))!;

		string ownerUsername = (await usersService.GetUserByIdAsync(new()
								   {
									   Id = headPost.Author.ToString()
								   })).Username;

		List<Post> newPostChains = await dbContext.Posts
												  .Where(p => p.Parent == headPost)
												  .OrderByDescending(p => p.LastUpdateDate)
												  .Take(5)
												  .ToListAsync();


		List<ProtoPost> postsToReturn = [];
		postsToReturn.AddRange(await MapPostsListToProtoPostsList(newPostChains));
		foreach(Post postchainHeadPost in newPostChains)
		{
			await AddPostHierarchyAsync(postchainHeadPost.Id, postsToReturn /*, 5*/);
		}

		ProtoTopic protoTopic = new()
		{
			Id = topic.Id.ToString(),
			Permalink = topic.Permalink,
			Forum = topic.Forum.ToString(),
			HeadPost = new()
			{
				Id = headPost.Id.ToString(),
				AuthorUsername = ownerUsername,
				Body = headPost.Body,
				Likes = headPost.Likes,
				Loves = headPost.Loves,
				Funnies = headPost.Funnies,
				Insights = headPost.Insights,
				Dislikes = headPost.Dislikes
			},
			Title = topic.Title
		};
		protoTopic.Posts.Add(postsToReturn);
		return protoTopic;
	}

	public override async Task<ProtoPost> GetHeadpostByTopic(GetHeadpostByTopicRequest request,
															 ServerCallContext context)
	{
		Guid topicId = new(request.Topic);
		Post post = await dbContext.Posts.FirstOrDefaultAsync(p => p.Parent == null && p.Topic.Id == topicId)
					?? throw new RpcException(new(StatusCode.NotFound, "No headpost was found with this topic ID"));

		return await MapPostToProtoPost(post);
	}

	public override async Task<GetRecentTopicsByForumReply> GetRecentTopicsByForum(
		GetRecentTopicsByForumRequest request,
		ServerCallContext callContext)
	{
		try
		{
			Guid forumId = new(request.Forum);
			List<Topic> topics = await dbContext.Topics.Where(t => t.Forum == forumId)
												.OrderByDescending(o => o.LastUpdateDate).Take(8)
												.ToListAsync();

			GetRecentTopicsByForumReply reply = new();
			reply.Topics.Add(await MapTopicsListToProtoTopicsList(topics, callContext));
			return reply;
		}
		catch(Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	public override async Task<CreateTopicReply> CreateTopic(CreateTopicRequest request, ServerCallContext callContext)
	{
		IEnumerable<Claim> accessTokenClaims = await ValidateAndGetUserClaims(request.AccessToken);
		Guid userId = Guid.Parse(accessTokenClaims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

		try
		{
			await _forumsService.GetForumById(new()
			{
				Id = request.Forum
			}, callContext);
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

	public override async Task<Empty> CreatePost(CreatePostRequest request, ServerCallContext callContext)
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


		// TODO: This should check if the post is the last child, to prevent having 2 posts having the same parent

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

	public override async Task<Empty> ReactOnPost(ReactOnPostRequest request,
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
		if(request.State is ReactionUpdateState.None && reaction is null)
		{
			return new();
		}

		if(request.State is ReactionUpdateState.None && reaction is not null)
		{
			dbContext.Remove(reaction);
			await dbContext.SaveChangesAsync();
			return new();
		}

		if(reaction is not null)
		{
			switch(reaction.ReactionType)
			{
				case ReactionType.Like:
					post.Likes--;
					break;
				case ReactionType.Love:
					post.Loves--;
					break;
				case ReactionType.Funny:
					post.Funnies--;
					break;
				case ReactionType.Insightful:
					post.Insights--;
					break;
				case ReactionType.Dislike:
					post.Dislikes--;
					break;
				default:
					throw new NotImplementedException();
			}
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

		switch(reaction.ReactionType)
		{
			case ReactionType.Like:
				post.Likes++;
				break;
			case ReactionType.Love:
				post.Loves++;
				break;
			case ReactionType.Funny:
				post.Funnies++;
				break;
			case ReactionType.Insightful:
				post.Insights++;
				break;
			case ReactionType.Dislike:
				post.Dislikes++;
				break;
			default:
				throw new NotImplementedException();
		}

		dbContext.Update(reaction);
		await dbContext.SaveChangesAsync();

		return new();
	}

	public override async Task<GetMorePostChainsReply> GetMorePostChains(GetMorePostChainsRequest request,
																		 ServerCallContext callContext)
	{
		if(!Guid.TryParse(request.Topic, out Guid topicGuid))
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Topic\" is not a valid GUID"));
		}

		if(request.SeenPostchains.Any(postchainHeadPost => !Guid.TryParse(postchainHeadPost, out _)))
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Guids provided are not valid"));
		}

		await ValidateAndGetUserClaims(request.AccessToken);


		Post topicHeadPost =
			await dbContext.Posts.FirstOrDefaultAsync(p => p.Parent == null && p.Topic.Id == topicGuid) ??
			throw new RpcException(new(StatusCode.NotFound, "The topic with this ID was not found"));


		List<Guid> seenPostchainsGuids = request.SeenPostchains.Select(Guid.Parse).ToList();

		List<Post> newPostChains = await dbContext.Posts
												  .OrderByDescending(p => p.LastUpdateDate)
												  .Where(p => !seenPostchainsGuids.Contains(p.Id) &&
															  p.Parent == topicHeadPost)
												  .Take(10)
												  .ToListAsync();


		List<ProtoPost> postsToReturn = [];
		postsToReturn.AddRange(await MapPostsListToProtoPostsList(newPostChains));
		foreach(Post postchainHeadPost in newPostChains)
		{
			await AddPostHierarchyAsync(postchainHeadPost.Id, postsToReturn /*, 5*/);
		}

		GetMorePostChainsReply reply = new();
		reply.Posts.Add(postsToReturn);
		return reply;
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

	private async Task AddPostHierarchyAsync(Guid parentId, ICollection<ProtoPost> posts /*, int depth*/)
	{
		/*if(depth <= 0)
		{
			return;
		}*/

		Post? child = await dbContext.Posts.Include(p => p.Parent)
									 .FirstOrDefaultAsync(p => p.Parent != null && p.Parent.Id == parentId);
		if(child != null)
		{
			ProtoPost postToAdd = await MapPostToProtoPost(child);

			posts.Add(postToAdd);

			await AddPostHierarchyAsync(child.Id, posts /*, depth - 1*/);
		}
	}

	private async Task<ProtoPost> MapPostToProtoPost(Post post)
	{
		string ownerUsername = (await usersService.GetUserByIdAsync(new()
								   {
									   Id = post.Author.ToString()
								   })).Username;

		ProtoPost protoPost = new()
		{
			Id = post.Id.ToString(),
			Body = post.Body,
			Likes = post.Likes,
			Loves = post.Loves,
			Insights = post.Insights,
			Funnies = post.Funnies,
			Dislikes = post.Dislikes,
			AuthorUsername = ownerUsername
		};

		if(post.Parent is not null)
		{
			protoPost.Parent = post.Parent.Id.ToString();
		}

		if(!string.IsNullOrWhiteSpace(post.Attachment))
		{
			protoPost.Attachment = post.Attachment;
		}

		return protoPost;
	}

	private async Task<List<ProtoPost>> MapPostsListToProtoPostsList(List<Post> posts)
	{
		List<ProtoPost> protoPosts = [];

		foreach(Post post in posts)
		{
			protoPosts.Add(await MapPostToProtoPost(post));
		}

		return protoPosts;
	}

	private async Task<ProtoTopic> MapTopicToProtoTopic(Topic topic, ServerCallContext callContext)
	{
		ProtoTopic protoTopic = new()
		{
			Id = topic.Id.ToString(),
			Permalink = topic.Permalink,
			Title = topic.Title,
			Forum = topic.Forum.ToString(),
			HeadPost = await GetHeadpostByTopic(new()
			{
				Topic = topic.Id.ToString()
			}, callContext)
		};

		return protoTopic;
	}

	private async Task<List<ProtoTopic>> MapTopicsListToProtoTopicsList(List<Topic> topics,
																		ServerCallContext callContext)
	{
		List<ProtoTopic> protoTopics = [];

		foreach(Topic topic in topics)
		{
			protoTopics.Add(await MapTopicToProtoTopic(topic, callContext));
		}

		return protoTopics;
	}

	#endregion
}