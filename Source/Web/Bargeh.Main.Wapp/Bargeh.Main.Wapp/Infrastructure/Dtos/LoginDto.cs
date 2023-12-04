using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos;

public class LoginDto
{
    [Required (ErrorMessage = "لطفا شماره‌ی همراهت رو وارد کن")]
    public string? Phone { get; set; }

    [Required (ErrorMessage = "لطفا رمز عبورت رو هم وارد کن")]
    [DataType (DataType.Password)]
    public string? Password { get; set; }

}