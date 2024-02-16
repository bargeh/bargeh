using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Client.Infrastructure.Dtos.Login;

public class VerificationDto : AuthenticationDtoBase
{
	//[Required (ErrorMessage = "لطفا کد فعال‌سازی ارسال شده رو وارد کن")]
	[Length(4, 4, ErrorMessage = "کد تایید باید ۴ رقمی باشه")]
	public string? VerificationCode { get; set; }
}