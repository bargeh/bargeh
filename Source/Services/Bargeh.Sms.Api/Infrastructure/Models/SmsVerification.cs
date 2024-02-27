namespace Bargeh.Sms.Api.Infrastructure.Models;

public class SmsVerification
{
	public Guid Id { get; init; }
	public required ushort Code { get; init; }
	// ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
	public required string Phone { get; init; }
	public required DateTime ExpireDate { get; init; }
}