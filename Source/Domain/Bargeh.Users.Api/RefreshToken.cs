using System.ComponentModel.DataAnnotations;

namespace Bargeh.Users.Api.Domain;

public sealed class RefreshToken
{
	public Guid Id { get; init; }

	[MaxLength(128)]
	public required string Token { get; init; }

	public required Guid UserId { get; init; }
	public DateTime ExpireDate { get; init; } = DateTime.UtcNow.AddDays(30);

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
