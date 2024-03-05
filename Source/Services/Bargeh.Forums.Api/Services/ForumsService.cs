using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Forums.Api.Infrastructure.Models;
using Forums.Api;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Forums.Api.Services;

public class ForumsService(ForumsDbContext dbContext) : ForumsProto.ForumsProtoBase
{
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
}