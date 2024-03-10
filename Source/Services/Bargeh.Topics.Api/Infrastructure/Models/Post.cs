using System.ComponentModel.DataAnnotations;

namespace Bargeh.Topics.Api.Infrastructure.Models;

public class Post
{
	public Guid Id { get; init; }
	public required Topic Topic { get; init; }

	[MaxLength(1024)]
	public string? Attachment { get; init; }

	[MaxLength(1024)]
	public string? Media { get; init; }

	public required Guid Author { get; init; }
	public DateTime LastUpdateDate { get; init; } = DateTime.UtcNow;

	[MaxLength(4096)]
	public required string Body { get; init; }

	public uint Likes { get; init; }
	public uint Loves { get; init; }
	public uint Funnies { get; init; }
	public uint Insights { get; init; }
	public uint Dislikes { get; init; }
}