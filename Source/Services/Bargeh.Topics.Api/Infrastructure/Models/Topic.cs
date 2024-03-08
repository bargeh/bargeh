using System.ComponentModel.DataAnnotations;

namespace Bargeh.Topics.Api.Infrastructure.Models;

public class Topic
{
	public Guid Id { get; init; }
	public required Guid ForumId { get; init; }
	public required Guid AuthorId { get; init; }

	[MaxLength(64)]
	public required string Title { get; init; }

	[MaxLength(4096)]
	public required string Body { get; init; }

	public uint Likes { get; init; }
	public uint Loves { get; init; }
	public uint Funnies { get; init; }
	public uint Insights { get; init; }
	public uint Dislikes { get; init; }
}