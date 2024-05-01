using System.ComponentModel.DataAnnotations;

namespace Bargeh.Users.Api.Infrastructure.Models;

public sealed class RefreshToken
{
	public Guid Id { get; init; }

	[MaxLength(128)]
	public required string Token { get; init; }

	public required Guid UserId { get; init; }
	public DateTime ExpireDate { get; init; } = DateTime.UtcNow.AddDays(30);
}