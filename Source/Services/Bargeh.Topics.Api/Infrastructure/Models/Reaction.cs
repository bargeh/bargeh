namespace Bargeh.Topics.Api.Infrastructure.Models;

public class Reaction
{
	public Guid Id { get; init; }
	public required Guid UserId { get; init; }
	public required Post Post { get; init; }
	public required ReactionType ReactionType { get; set; }
}

public enum ReactionType
{
	Like,
	Love,
	Funny,
	Insightful,
	Dislike
}