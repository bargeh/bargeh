using System.ComponentModel.DataAnnotations;

namespace Bargeh.Forums.Api.Infrastructure.Models;

public class Topic
{
	public Topic()
	{
		Id = Guid.NewGuid();
		Permalink = Id.ToString().Split('-')[0];
	}

	public Guid Id { get; init; }
	public required Guid Forum { get; init; }

	[MaxLength(16)]
	public string Permalink { get; init; }

	[MaxLength(64)]
	public required string Title { get; init; }

	public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
}