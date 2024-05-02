using System.ComponentModel.DataAnnotations;

namespace Bargeh.Forums.Api.Infrastructure.Models;

public class Post
{
	public Guid Id { get; init; }
	public required Topic Topic { get; init; }
	public Post? Parent { get; init; }

	[MaxLength(1024)]
	public string? Attachment { get; init; }

	public required Guid Author { get; init; }
	public DateTime LastUpdateDate { get; init; } = DateTime.UtcNow;

	[MaxLength(4096)]
	public required string Body { get; init; }

	public uint Likes { get; set; }
	public uint Loves { get; set; }
	public uint Funnies { get; set; }
	public uint Insights { get; set; }
	public uint Dislikes { get; set; }
}