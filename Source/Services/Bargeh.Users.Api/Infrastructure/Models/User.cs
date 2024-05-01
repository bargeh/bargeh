// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace Bargeh.Users.Api.Infrastructure.Models;

public class User
{
	public Guid Id { get; init; } = Guid.NewGuid ();
	public required string Username { get; init; }
	public required string DisplayName { get; init; }
	public required string PhoneNumber { get; init; }
	public string? Password { get; set; }
	public string Bio { get; init; } = string.Empty;
	public string ProfileImage { get; init; } = "Default.webp";
	public string Cover { get; init; } = "Cover.webp";
	public ushort PremiumDaysLeft { get; init; }

	public DateTime OnlineDate { get; init; } = DateTime.UtcNow;
	public DateTime RegisterDate { get; init; } = DateTime.UtcNow;
	public bool Enabled { get; set; } = true;
	public bool CanCreateForums { get; init; } = true;
	public string? Email { get; init; }
}