namespace Bargeh.Identity.Api.Models;

public sealed class RefreshToken
{
	public Guid Id { get; init; } = new ();
	public required string Token { get; init; }
	public required Guid UserId { get; init; }
	public DateTime ExpireDate { get; init; } = DateTime.UtcNow.AddDays (30);
}
