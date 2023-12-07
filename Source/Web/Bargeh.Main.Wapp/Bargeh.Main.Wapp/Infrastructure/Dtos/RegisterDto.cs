using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos;

public class RegisterDto : AuthenticationDtoBase
{
    [Required (ErrorMessage = "لطفا یه نام کاربری برای خودت انتخاب کن")]
    [Length (6, 20, ErrorMessage = "نام کاربری باید بین ۶ تا ۲۰ کارکتر باشه")]
    public string? Username { get; set; }

    [Required (ErrorMessage = "لطفا یه نام نمایشی هم برای خودت انتخاب کن")]
    [Length (1, 30, ErrorMessage = "نام نمایشی باید بین ۱ تا ۳۰ کارکتر باشه")]
    public string? DisplayName { get; set; }

    [Range (typeof(bool), "true", "true", ErrorMessage = "لطفا شرایط استفاده از برگه رو هم بخون و قبول کن")]
    public bool AcceptedTos { get; set; } = false;
}