using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Client.Infrastructure.Dtos.Login;

public class VerificationDto : AuthenticationDtoBase
{
	[Length(4, 4, ErrorMessage = "کد تایید باید ۴ رقمی باشه")]
	public string? VerificationCode { get; set; }
}