namespace Bargeh.Forums.Api.Infrastructure.Models;

public class Report
{
	public Guid Id { get; init; }
	public required Post Post { get; init; }
	public required Guid UserId { get; init; }
}