using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Forums.Api.Infrastructure.Models;
using Bargeh.Users.Api;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bargeh.Admin.Api.Services;

public class AdminService(UsersProto.UsersProtoClient usersService, ForumsDbContext forumsDbContext)
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

	public override async Task<Empty> DeleteForum(DeleteForumRequest request, ServerCallContext context)
	{
		await ValidateAndGetUserClaims(request.AccessToken);

		return new();
	}

	public override async Task<ProtoReportsList> GetReports(Empty request, ServerCallContext context)
	{
		// TODO: Should get access token
		List<Report> reports = await forumsDbContext.Reports.Include(r => r.Post).ToListAsync();

		ProtoReportsList protoReportsList = new();
		protoReportsList.Reports.Add(MapReportToProtoReport(reports));

		return protoReportsList;
	}


	public override async Task<Empty> AcceptReport(ReportReviewRequest request, ServerCallContext context)
	{
		await ValidateAndGetUserClaims(request.AccessToken);

		if(!Guid.TryParse(request.ReportId, out Guid reportId))
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter ID is not valid"));

		Report report = await forumsDbContext.Reports.Include(r => r.Post).FirstOrDefaultAsync(r => r.Id == reportId)
						?? throw new RpcException(new(StatusCode.NotFound, "No report was found with this ID"));

		forumsDbContext.Remove(report.Post);
		forumsDbContext.Remove(report);
		await forumsDbContext.SaveChangesAsync();

		return new();
	}

	public override async Task<Empty> DeclineReport(ReportReviewRequest request, ServerCallContext context)
	{
		await ValidateAndGetUserClaims(request.AccessToken);

		if(!Guid.TryParse(request.ReportId, out Guid reportId))
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter ID is not valid"));

		Report report = await forumsDbContext.Reports.Include(r => r.Post).FirstOrDefaultAsync(r => r.Id == reportId)
						?? throw new RpcException(new(StatusCode.NotFound, "No report was found with this ID"));

		forumsDbContext.Remove(report);
		await forumsDbContext.SaveChangesAsync();

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

	private static RepeatedField<ProtoReport> MapReportToProtoReport(List<Report> reports)
	{
		RepeatedField<ProtoReport> protoReports = new();

		foreach(ProtoReport protoReport in reports.Select(report => new ProtoReport
				{
					Id = report.Id.ToString(),
					PostBody = report.Post.Body
				}))
			protoReports.Add(protoReport);

		return protoReports;
	}
}