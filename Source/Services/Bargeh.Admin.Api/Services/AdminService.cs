using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Bargeh.Users.Api;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;

namespace Bargeh.Admin.Api.Services;

public class AdminService(UsersProto.UsersProtoClient usersService)
	: AdminProto.AdminProtoBase
{
	public override async Task<Empty> AddUser(AddUserRequest request, ServerCallContext context)
	{
		await ValidateAndGetUserClaims(request.AccessToken);

		try
		{
			await usersService.AddUserAsync(new()
			{
				Username = request.Username,
				DisplayName = request.DisplayName,
				Phone = request.PhoneNumber,
				AcceptedTos = true
			});
		}
		catch(Exception exception)
		{
			throw new RpcException(new(StatusCode.Internal, JsonSerializer.Serialize(exception)));
		}

		return new();
	}

	public override async Task<Empty> DisableUser(DisableUserRequest request, ServerCallContext context)
	{
		await ValidateAndGetUserClaims(request.AccessToken);
		
		try
		{
			await usersService.DisableUserAsync(new()
			{
				Id = request.UserId
			});
		}
		catch(Exception exception)
		{
			throw new RpcException(new(StatusCode.Internal, JsonSerializer.Serialize(exception)));
		}

		return new();
	}

	private static async Task ValidateAndGetUserClaims(string accessToken)
	{
		JwtSecurityTokenHandler tokenHandler = new();
		SecurityKey key = new X509SecurityKey(new("C:/Source/Bargeh/JwtPublicKey.cer"));

		TokenValidationResult tokenValidationResult =
			await tokenHandler.ValidateTokenAsync(accessToken, new()
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = key,
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidIssuer = "https://bargeh.net",
				ValidAudience = "https://bargeh.net",
				ClockSkew = TimeSpan.Zero
			});

		if(!tokenValidationResult.IsValid)
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"AccessToken\" is not valid"));
		}

		IEnumerable<Claim> accessTokenClaims = tokenHandler.ReadJwtToken(accessToken).Claims!;

		Claim? uniqueName = accessTokenClaims.FirstOrDefault(c => c.Type == "unique_name");

		if(uniqueName is null || uniqueName.Value != "matin")
		{
			throw new RpcException(new(StatusCode.Unauthenticated, "Why should you be able to access this API?"));
		}
	}
}