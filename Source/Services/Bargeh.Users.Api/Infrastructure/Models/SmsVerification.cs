using System.ComponentModel.DataAnnotations;

namespace Bargeh.Users.Api.Infrastructure.Models;

public class SmsVerification
{
	public Guid Id { get; init; }
	public required ushort Code { get; init; }

	[MaxLength(11)]
	public required string Phone { get; init; }

	public required DateTime ExpireDate { get; init; }
}