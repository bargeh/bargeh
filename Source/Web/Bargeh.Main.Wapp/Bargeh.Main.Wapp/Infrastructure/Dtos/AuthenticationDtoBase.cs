using MatinDevs.PersianPhoneNumbers;
using System.ComponentModel.DataAnnotations;

namespace Bargeh.Main.Wapp.Infrastructure.Dtos;

public class AuthenticationDtoBase
{
    [Required (ErrorMessage = "لطفا شماره‌ی همراهت رو وارد کن")]
    public string? Phone { get; set; }

    public bool PhoneValid => Phone != null && Phone.IsPersianPhoneValid ();
}