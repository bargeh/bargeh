namespace Bargeh.Sms.Api.Models;

public class SmsVerification
{
	public Guid Id { get; set; }
	public required ushort Code { get; set; }
	public required Guid UserId { get; set; }
}