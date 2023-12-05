using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos;

public class RegisterDto
{
    [Required (ErrorMessage = "لطفا شماره‌ی همراهت رو وارد کن")]
    public string? Phone { get; set; }

    [Required (ErrorMessage = "لطفا یه نام کاربری برای خودت انتخاب کن")]
    [Length (6, 20, ErrorMessage = "نام کاربری باید بین ۶ تا ۲۰ کارکتر باشه")]
    public string? Username { get; set; }

    [Required (ErrorMessage = "لطفا یه نام نمایشی هم برای خودت انتخاب کن")]
    [Length (1, 30, ErrorMessage = "نام نمایشی باید بین ۱ تا ۳۰ کارکتر باشه")]
    public string? DisplayName { get; set; }
}