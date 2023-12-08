using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos.Login;

public class SetPasswordDto
{
	[Required (ErrorMessage = "لطفا یه رمز عبور وارد کن")]
	[Length (8, 72, ErrorMessage = "رمز عبور باید بین ۸ و ۷۲ کارکتر باشه")]
	public string? Password { get; set; }

	[Required (ErrorMessage = "لطفا تایید رمز عبور هم وارد کن")]
	[Compare (nameof (Password), ErrorMessage = "رمز عبورهایی که وارد کردی مثل هم نبودن. لطفا دوباره واردشون کن")]
	public string? ConfirmPassword { get; set; }
}
