using System.Security.Claims;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Users.API;

namespace Bargeh.Main.Wapp.Controllers;

public class UserController : Controller
{
    [HttpPost]
    [Route ("/Api/Login")]
    // [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login ([FromHeader] string phone,
                                           [FromHeader] string password,
                                           [FromHeader] string captcha)
    {
        // const string verifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        // HttpClient httpClient = new ();
        //
        // var content = new FormUrlEncodedContent (new[]
        // {
        // 	new KeyValuePair<string, string> ("secret", "6LcNsqsoAAAAAII-RQ6b0D-kpSHaN7wKgYStYSta"),
        // 	new KeyValuePair<string, string> ("response", captcha),
        // 	new KeyValuePair<string, string> ("remoteip", HttpContext.Connection?.RemoteIpAddress.ToString ())
        // });

        // var response = await httpClient.PostAsync (verifyUrl, content);

        var client = new UsersProto.UsersProtoClient (GrpcChannel.ForAddress ("http://usersapi"));

        try
        {
            var user = await client.GetUserByPhoneAndPasswordAsync (new GetUserByPhoneAndPasswordRequest
            {
                Phone = phone,
                Password = password.Hash (HashType.SHA256)
            });

            if (!user.Enabled) return Forbid ();

            await HttpContext.SignOutAsync ();

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Username),
                new ("Email", user.Email),
                new ("DisplayName", user.DisplayName)
            };

            var claimsIdentity = new ClaimsIdentity (
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.Now,
                RedirectUri = "/"
            };

            await HttpContext.SignInAsync (
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal (claimsIdentity),
                authProperties);

            return Ok ();
        }
        catch (RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case Grpc.Core.StatusCode.NotFound:
                    return NotFound ();
                case Grpc.Core.StatusCode.PermissionDenied:
                    return Forbid ();
                // TODO: Throw proper status code for invalid captcha
                default:
                    return Ok ();
            }
        }
    }

    [HttpPost]
    [Route ("/Api/Logout")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Logout ()
    {
        await HttpContext.SignOutAsync ();
        return Ok ();
    }
}