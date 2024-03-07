namespace Bargeh.Forums.Api.Infrastructure.Models;

public class ForumMembership
{
	public Guid Id { get; init; }
	public required Guid UserId { get; init; }
	public required Forum Forum { get; init; }
}