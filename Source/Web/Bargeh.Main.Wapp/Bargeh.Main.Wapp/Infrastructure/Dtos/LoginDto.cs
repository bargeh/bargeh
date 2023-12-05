using System.ComponentModel.DataAnnotations;
using MatinDevs.PersianPhoneNumbers;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos;

public class LoginDto : AuthenticationDtoBase
{
    [Required (ErrorMessage = "لطفا رمز عبورت رو هم وارد کن")]
    [DataType (DataType.Password)]
    public string? Password { get; set; }
}