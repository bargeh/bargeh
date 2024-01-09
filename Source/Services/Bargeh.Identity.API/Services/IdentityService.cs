using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Bargeh.Identity.Api.Infrastructure;
using Bargeh.Identity.Api.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Identity.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Users.Api;
using LoginRequest = Identity.Api.LoginRequest;
using RefreshRequest = Identity.Api.RefreshRequest;

namespace Bargeh.Identity.Api.Services;

public class IdentityService 
    (IConfiguration configuration, IdentityDbContext dbContext, TimeProvider timeProvider) 
    : IdentityProto.IdentityProtoBase
{
	private readonly UsersProto.UsersProtoClient _usersApiClient =
		new (GrpcChannel.ForAddress (configuration.GetValue<string> ("services:users:1")!));

	#region Grpc Endpoints

	public override async Task<TokenResponse> Login (LoginRequest request, ServerCallContext callContext)
	{
		GetUserReply user = await _usersApiClient.GetUserByPhoneAndPasswordAsync (new ()
		{
			Phone = request.Phone,
			Password = request.Password,
			Captcha = request.Captcha
		});

		string jwtToken = GenerateJwt (user);
		string refreshToken = await GenerateRefreshToken (Guid.Parse (user.Id));

		return new ()
		{
			JwtToken = jwtToken,
			RefreshToken = refreshToken
		};
	}

	public override async Task<TokenResponse> Refresh (RefreshRequest request, ServerCallContext callContext)
	{
		RefreshToken oldToken = await dbContext.RefreshTokens.FirstOrDefaultAsync (r => r.Token == request.OldRefreshToken)
			?? throw new RpcException (new (StatusCode.NotFound, "The refresh token was not found"));

		if (oldToken.ExpireDate <= timeProvider.GetUtcNow ())
		{
			dbContext.Remove (oldToken);
			await dbContext.SaveChangesAsync ();
			throw new RpcException (new (StatusCode.FailedPrecondition, "The token is expired"));
		}

		GetUserReply? user = _usersApiClient.GetUserById (new ()
		{
			Id = oldToken.UserId.ToString ()
		});

		if (oldToken.ExpireDate >= timeProvider.GetUtcNow ().AddMinutes (-4))
		{
			await _usersApiClient.DisableUserAsync (new ()
			{
				Id = user.Id
			});

			dbContext.Remove (oldToken);
			await dbContext.SaveChangesAsync ();
			throw new RpcException (new (StatusCode.Internal, "Internal Error"));
		}

		Guid userId = Guid.Parse (user.Id);

		if (!user.Enabled)
		{
			throw new RpcException (new (StatusCode.PermissionDenied, "The user can not get refresh token"));
		}

		string newToken = await GenerateRefreshToken (userId);

		dbContext.Remove (oldToken);
		dbContext.Add (new RefreshToken
		{
			Token = newToken,
			UserId = userId
		});

		await dbContext.SaveChangesAsync ();

		string jwtToken = GenerateJwt (user);

		return new ()
		{
			JwtToken = jwtToken,
			RefreshToken = newToken
		};
	}

	#endregion

	#region Static Methods

	private static string GenerateJwt (GetUserReply user)
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
			Token = token,
		};

		dbContext.RefreshTokens.Add (refreshToken);
		await dbContext.SaveChangesAsync ();

		return token;
	}

	#endregion
}