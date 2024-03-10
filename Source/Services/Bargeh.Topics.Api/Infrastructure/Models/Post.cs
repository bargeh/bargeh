using System.ComponentModel.DataAnnotations;

namespace Bargeh.Topics.Api.Infrastructure.Models;

public class Post
{
	public Guid Id { get; set; }
	public required Topic Topic { get; set; }
	public string? Attachment { get; set; }
	public string? Media { get; set; }
	public required Guid Author { get; set; }
	public DateTime LastUpdateDate { get; init; } = DateTime.UtcNow;

	[MaxLength(4096)]
	public required string Body { get; init; }

	public uint Likes { get; init; }
	public uint Loves { get; init; }
	public uint Funnies { get; init; }
	public uint Insights { get; init; }
	public uint Dislikes { get; init; }
}