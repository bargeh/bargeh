using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Bargeh.Identity.API.Infrastructure;
using Bargeh.Identity.API.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Identity.API;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Users.API;
using LoginRequest = Identity.API.LoginRequest;

namespace Bargeh.Identity.API.Services;

public class IdentityService (IConfiguration configuration, IdentityDbContext context) : IdentityProto.IdentityProtoBase
{
	#region Grpc Endpoints

	public override async Task<LoginResponse> Login (LoginRequest request, ServerCallContext callContext)
	{

		//FROMHERE: Make db work
		string usersApiAddress = configuration.GetValue<string> ("services:usersapi:1")!;
		GrpcChannel usersApiChannel = GrpcChannel.ForAddress (usersApiAddress);
		UsersProto.UsersProtoClient client = new (usersApiChannel);
		GetUserReply user = new ();

		try
		{
			user = await client.GetUserByPhoneAndPasswordAsync (new ()
			{
				Phone = request.Phone,
				Password = request.Password,
				Captcha = request.Captcha
			});
		}
		catch (RpcException exception)
		{
			if (exception.Status.StatusCode == StatusCode.NotFound)
			{
				throw new RpcException (new (StatusCode.NotFound,
					"The user with this phone number and password was not found"));
			}
		}

		string jwtToken = GenerateJwt (user);
		string refreshToken = await GenerateRefreshToken (Guid.Parse(user.Id));

		return new ()
		{
			JwtToken = jwtToken,
			RefreshToken = refreshToken
		};
	}

	#endregion

	#region Static Methods

	private string GenerateJwt (GetUserReply user)
	{
		RsaSecurityKey securityKey = new (new X509Certificate2 ("C:/Users/Matin/Desktop/private_key.pfx", "bargeh.dev").GetRSAPrivateKey ());

		SigningCredentials credentials = new (securityKey, SecurityAlgorithms.RsaSha256);

		List<Claim> claims =
		[
			new (ClaimsIdentity.DefaultNameClaimType, user.Phone),
			new (JwtRegisteredClaimNames.Email, user.Email),
			new (JwtRegisteredClaimNames.GivenName, user.DisplayName),
			new (JwtRegisteredClaimNames.UniqueName, user.Username),
			new ("premiumDaysLeft", user.PremiumDaysLeft.ToString()),
			new ("avatar", user.ProfileImage)
		];

		JwtSecurityToken jwtToken = new (
			issuer: "https://bargeh.net",
			audience: "https://bargeh.net",
			claims,
			expires: DateTime.UtcNow.AddMinutes (5),
			signingCredentials: credentials);

		string token = new JwtSecurityTokenHandler ().WriteToken (jwtToken);

		return token;
	}

	private async Task<string> GenerateRefreshToken (Guid userId)
	{
		const short length = 64;
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

		string token = new (Enumerable.Repeat (chars, length)
			.Select (s => s[Random.Shared.Next (s.Length)]).ToArray ());

		RefreshToken refreshToken = new ()
		{
			UserId = userId,
			Token = token
		};

		context.RefreshTokens.Add (refreshToken);
		await context.SaveChangesAsync ();

		return token;
	}

	#endregion
}