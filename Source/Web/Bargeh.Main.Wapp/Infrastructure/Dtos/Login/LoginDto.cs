using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos.Login;

public class LoginDto : AuthenticationDtoBase
{
	[Required (ErrorMessage = "لطفا رمز عبورت رو هم وارد کن")]
	[DataType (DataType.Password)]
	public string? Password { get; set; }
}