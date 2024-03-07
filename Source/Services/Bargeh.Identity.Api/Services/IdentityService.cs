using Bargeh.Identity.Api.Infrastructure;
using Grpc.Core;
using Identity.Api;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Bargeh.Identity.Api.Infrastructure.Models;
using Users.Api;
using LoginRequest = Identity.Api.LoginRequest;
using RefreshRequest = Identity.Api.RefreshRequest;

namespace Bargeh.Identity.Api.Services;

public class IdentityService(
	UsersProto.UsersProtoClient usersApiClient,
	IdentityDbContext dbContext,
	TimeProvider timeProvider)
	: IdentityProto.IdentityProtoBase
{
	#region Grpc Endpoints

	public override async Task<TokenResponse> Login(LoginRequest request, ServerCallContext callContext)
	{
		GetUserReply user = default!;

		try
		{
			user = await usersApiClient.GetUserByPhoneAndPasswordAsync(new()
			{
				Phone = request.Phone,
				Password = request.Password,
				Captcha = request.Captcha
			});
		}
		catch(RpcException exception)
		{
			if(exception.StatusCode == StatusCode.NotFound)
			{
				throw new RpcException(new(StatusCode.NotFound, "The user with this info was not found"));
			}
		}

		string accessToken = GenerateAccessToken(user);
		string refreshToken = await GenerateRefreshToken(Guid.Parse(user.Id));

		return new()
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}

	public override async Task<TokenResponse> Refresh(RefreshRequest request, ServerCallContext callContext)
	{
		RefreshToken oldRefreshToken = await dbContext.GetRefreshTokenByOldToken(request.OldRefreshToken)
									   ?? throw new RpcException(new(StatusCode.NotFound,
																	 "The refresh token was not found"));

		if(oldRefreshToken.ExpireDate <= timeProvider.GetUtcNow())
		{
			dbContext.Remove(oldRefreshToken);
			await dbContext.SaveChangesAsync();
			throw new RpcException(new(StatusCode.FailedPrecondition, "The token is expired"));
		}

		GetUserReply? user = usersApiClient.GetUserById(new()
		{
			Id = oldRefreshToken.UserId.ToString()
		});

		if(oldRefreshToken.ExpireDate >= timeProvider.GetUtcNow().AddMinutes(-4))
		{
			await usersApiClient.DisableUserAsync(new()
			{
				Id = user.Id
			});

			dbContext.Remove(oldRefreshToken);
			await dbContext.SaveChangesAsync();
			throw new RpcException(new(StatusCode.Internal, "Internal Error"));
		}

		Guid userId = Guid.Parse(user.Id);

		if(!user.Enabled)
		{
			throw new RpcException(new(StatusCode.PermissionDenied, "The user can not get refresh token"));
		}

		string newToken = await GenerateRefreshToken(userId);

		dbContext.Remove(oldRefreshToken);
		dbContext.Add(new RefreshToken
		{
			Token = newToken,
			UserId = userId
		});

		await dbContext.SaveChangesAsync();

		string accessToken = GenerateAccessToken(user);

		return new()
		{
			AccessToken = accessToken,
			RefreshToken = newToken
		};
	}

	#endregion

	#region Static Methods

	private static string GenerateAccessToken(GetUserReply user)
	{
		// PRODUCTION: Make a real key
		RsaSecurityKey securityKey = new(new X509Certificate2("C:/Source/Bargeh/JwtPrivateKey.pfx", "bargeh.dev")
											 .GetRSAPrivateKey());

		SigningCredentials credentials = new(securityKey, SecurityAlgorithms.RsaSha256);

		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, user.Id),
			new(ClaimsIdentity.DefaultNameClaimType, user.Phone),
			new(JwtRegisteredClaimNames.Email, user.Email),
			new(JwtRegisteredClaimNames.GivenName, user.DisplayName),
			new(JwtRegisteredClaimNames.UniqueName, user.Username),
			new("premiumDaysLeft", user.PremiumDaysLeft.ToString()),
			new("avatar", user.ProfileImage)
		];

		JwtSecurityToken accessToken = new(
										   issuer: "https://bargeh.net",
										   audience: "https://bargeh.net",
										   claims,
										   expires: DateTime.UtcNow.AddMinutes(5),
										   signingCredentials: credentials);

		string token = new JwtSecurityTokenHandler().WriteToken(accessToken);

		return token;

		// ReSharper disable CommentTypo
		// The never expiring token:
		// eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ODQ0ZmQ0Ny0zMjM2LTQ2Y2ItODk4ZC02MDdiNWM1NTYzYzEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiIiwiZW1haWwiOiJ0ZXN0QGdtYWlsLmJhcmdlaCIsImdpdmVuX25hbWUiOiJ0ZXN0IGRpc3BsYXkgbmFtZSIsInVuaXF1ZV9uYW1lIjoidGVzdCIsInByZW1pdW1EYXlzTGVmdCI6IjAiLCJhdmF0YXIiOiJEZWZhdWx0LndlYnAiLCJleHAiOjI1MzQwMjI4ODIwMCwiaXNzIjoiaHR0cHM6Ly9iYXJnZWgubmV0IiwiYXVkIjoiaHR0cHM6Ly9iYXJnZWgubmV0In0.udWSZliBPWnoS0TL7P-XTW6x4QN_WLaXn3gaiZZfvIeU6gdufDDmkLlajes10C-UDKNQ2YBu2mCRj97f5acax1vL5qBFzmARnSaFIo8UzgLHPBH99TiPGh40HUY4qfnjtGcjpKKikHdV42svJHJiVzyZZE8bTu4Y6RtWqgIciKwWaAAyh6TnrW8iCgTe7Fdl29PGKq3mZQNFym66RqInabMZcDZ-pj1L9qNEnEvAwZFYBvlhXFOq27OSBGtiF9TM1dSSO8kRzYxXi9aKRSujvH1zmaFwIHegRXwAUP5dFH2HmGKPEsAbWpxTlGFHoSjEbCo5QH1Y0u_0RnayeFrFtQ
		// ReSharper restore CommentTypo
	}

	private async Task<string> GenerateRefreshToken(Guid userId)
	{
		const short length = 128;

		// ReSharper disable StringLiteralTypo
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

		// ReSharper restore StringLiteralTypo

		string token = new(Enumerable.Repeat(chars, length)
									 .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());

		RefreshToken refreshToken = new()
		{
			UserId = userId,
			Token = token
		};

		dbContext.Add(refreshToken);
		await dbContext.SaveChangesAsync();

		return token;
	}

	#endregion
}