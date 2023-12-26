namespace Bargeh.Identity.API.Models;

public sealed class RefreshToken
{
	public Guid Id { get; set; } = new ();
	public required string Token { get; set; }
	public required Guid UserId { get; set; }
	public DateTime ExpireDate { get; set; } = DateTime.UtcNow.AddDays (30);
}
