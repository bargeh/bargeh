using System.ComponentModel.DataAnnotations;
using MatinDevs.PersianPhoneNumbers;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos.Login;

public class AuthenticationDtoBase
{
    [Required (ErrorMessage = "لطفا شماره‌ی همراهت رو وارد کن")]
    public string? Phone { get; set; }

    public bool PhoneValid => Phone != null && Phone.IsPersianPhoneValid ();
}