using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Infrastructure.Models;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using MediatR;

namespace Bargeh.Users.Api.Services;

public class IdentityService(
	UsersDbContext dbContext,
	TimeProvider timeProvider,
	ILogger<UsersService> logger,
	IMediator mediator)
	: IdentityProto.IdentityProtoBase
{
	private readonly UsersService _usersService = new(dbContext, logger);

	#region Grpc Endpoints

	public override async Task<TokenResponse> Login(LoginRequest request, ServerCallContext callContext)
	{
		ProtoUser user = default!;

		try
		{
			user = await _usersService.GetUserByPhoneAndPassword(new()
			{
				Phone = request.Phone,
				Password = request.Password,
				Captcha = request.Captcha
			}, callContext);
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

		ProtoUser user = await _usersService.GetUserById(new()
		{
			Id = oldRefreshToken.UserId.ToString()
		}, callContext);

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

	private static string GenerateAccessToken(ProtoUser user)
	{
		const string keyPath = "/Users/matin/sources/Bagreh/JwtPrivateKey.pfx";

		RsaSecurityKey securityKey = new(new X509Certificate2(keyPath, "bargeh.dev")
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
			new("avatar", user.Avatar)
		];

		JwtSecurityToken accessToken = new(
										   issuer: "https://bargeh.net",
										   audience: "https://bargeh.net",
										   claims,
										   expires: DateTime.UtcNow.AddMinutes(5),
										   signingCredentials: credentials);

		string token = new JwtSecurityTokenHandler().WriteToken(accessToken);

		return token;
	}

	private async Task<string> GenerateRefreshToken(Guid userId)
	{
		const short length = 128;

		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

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
