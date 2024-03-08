using Bargeh.Topics.Api.Infrastructure;
using Bargeh.Topics.Api.Infrastructure.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Topics.Api;

namespace Bargeh.Topics.Api.Services;

public class TopicsService(TopicsDbContext dbContext) : TopicsProto.TopicsProtoBase
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
}