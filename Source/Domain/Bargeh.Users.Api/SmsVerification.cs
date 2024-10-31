using System.ComponentModel.DataAnnotations;

namespace Bargeh.Users.Api.Domain;

public class SmsVerification
{
	public Guid Id { get; init; }
	public required ushort Code { get; init; }

	[MaxLength(11)]
	public required string Phone { get; init; }

	public required DateTime ExpireDate { get; init; }

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
