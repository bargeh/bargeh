using System.ComponentModel.DataAnnotations;

namespace Bargeh.Topics.Api.Infrastructure.Models;

public class Topic
{
	public Guid Id { get; init; }
	public required Guid Forum { get; init; }

	[MaxLength(16)]
	public string Permalink { get; init; }

	[MaxLength(64)]
	public required string Title { get; init; }
	
	public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;

	public Topic()
	{
		Id = Guid.NewGuid();
		Permalink = Id.ToString().Split('-')[0];
	}
}