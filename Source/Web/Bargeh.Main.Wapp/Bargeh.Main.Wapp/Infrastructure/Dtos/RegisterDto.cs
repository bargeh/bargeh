using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos;

public class RegisterDto
{
    [Required (ErrorMessage = "لطفا شماره‌ی همراهت رو وارد کن")]
    public string? Phone { get; set; }

    [Required (ErrorMessage = "لطفا یه نام کاربری برای خودت انتخاب کن")]
    public string? Username { get; set; }

    [Required (ErrorMessage = "لطفا یه نام نمایشی هم برای خودت انتخاب کن")]
    public string? DisplayName { get; set; }
}