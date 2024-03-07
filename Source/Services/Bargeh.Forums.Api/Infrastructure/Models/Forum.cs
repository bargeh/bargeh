using System.ComponentModel.DataAnnotations;

namespace Bargeh.Forums.Api.Infrastructure.Models;

public class Forum
{
	public Guid Id { get; init; }

	[MaxLength(32)]
	public required string Name { get; init; }

	[MaxLength(350)]
	public required string Description { get; init; }

	[MaxLength(32)]
	public required string Permalink { get; init; }

	public uint Members { get; set; }

	public uint Supporters { get; init; }

	public required Guid OwnerId { get; init; }
}