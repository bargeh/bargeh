using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos;

public class VerificationDto
{
	[Required (ErrorMessage = "لطفا کد فعال‌سازی ارسال شده رو وارد کن")]
	[Length (4, 4, ErrorMessage = "کد تایید باید ۴ رقمی باشه")]
	public string? VerificationCode { get; set; }
}
