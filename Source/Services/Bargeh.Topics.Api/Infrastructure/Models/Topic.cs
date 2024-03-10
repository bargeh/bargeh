using System.ComponentModel.DataAnnotations;

namespace Bargeh.Topics.Api.Infrastructure.Models;

public class Topic
{
	public Guid Id { get; init; }
	public required Guid ForumId { get; init; }

	[MaxLength(64)]
	public string Permalink { get; init; } = "topic";

	[MaxLength(64)]
	public required string Title { get; init; }
}