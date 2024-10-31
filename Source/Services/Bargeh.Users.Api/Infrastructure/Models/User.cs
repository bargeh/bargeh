namespace Bargeh.Users.Api.Infrastructure.Models;

// PRODUCTION: Apply length limits for strings
public class User
{
	public Guid Id { get; init; }
	public required string Username { get; init; }
	public required string DisplayName { get; init; }
	public required string PhoneNumber { get; init; }
	public string? Password { get; set; }
	public string Bio { get; init; } = string.Empty;
	public string Avatar { get; init; } = "Profile";
	public string Cover { get; init; } = "CoverDefault";
	public uint Followers { get; set; }
	public ushort PremiumDaysLeft { get; init; }

	public DateTime OnlineDate { get; init; } = DateTime.UtcNow;
	public DateTime RegisterDate { get; init; } = DateTime.UtcNow;
	public bool Enabled { get; set; } = true;
	public bool CanCreateForums { get; init; } = true;
	public string? Email { get; init; }

	// DDD: Add domain events
	private readonly List<IDomainEvent> _domainEvents = new();
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

	public void AddDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

	public void RemoveDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Remove(domainEvent);
	}

	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
}
