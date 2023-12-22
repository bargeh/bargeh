using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

		UsersProto.UsersProtoClient client = new UsersProto.UsersProtoClient (GrpcChannel.ForAddress ("http://usersapi"));

		// FROMHERE: Implement JWT Authentication

		try
		{
			GetUserReply? user = await client.GetUserByPhoneAndPasswordAsync (new ()
			{
				Phone = phone,
				Password = password.Hash (HashType.SHA256)
			});

			if (!user.Enabled) return Forbid ();

			await HttpContext.SignOutAsync ();

			List<Claim> claims = new List<Claim>
			{
				new (ClaimTypes.Name, user.Username),
				new ("Email", user.Email),
				new ("DisplayName", user.DisplayName)
			};

			ClaimsIdentity claimsIdentity = new ClaimsIdentity (
				claims, CookieAuthenticationDefaults.AuthenticationScheme);

			AuthenticationProperties authProperties = new AuthenticationProperties
			{
				AllowRefresh = false,
				IsPersistent = true,
				IssuedUtc = DateTimeOffset.Now,
				RedirectUri = "/"
			};

			await HttpContext.SignInAsync (
				CookieAuthenticationDefaults.AuthenticationScheme,
				new (claimsIdentity),
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