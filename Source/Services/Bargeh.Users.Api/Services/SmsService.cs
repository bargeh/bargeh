using Bargeh.Users.Api.Infrastructure;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MatinDevs.PersianPhoneNumbers;
using MediatR;

namespace Bargeh.Users.Api.Services;

public class SmsService(UsersDbContext dbContext, ILogger<UsersService> logger, IMediator mediator) : SmsProto.SmsProtoBase
{
	private readonly UsersService _usersService = new(dbContext, logger);

	public override async Task<Empty> SendVerification(SendVerificationRequest request, ServerCallContext callContext)
	{
		bool phoneValid = request.Phone.IsPersianPhoneValid();

		if(!phoneValid)
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Phone\" is not valid"));
		}

		ProtoUser? user = await _usersService.GetUserByPhone(new()
		{
			Phone = request.Phone
		}, callContext);

		if(user is null)
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "User with parameter \"Phone\" was not found"));
		}

		ushort code = (ushort)Random.Shared.Next(1000, 9999);

		// PRODUCTION: Add a real SMS sending API contract here

		await dbContext.SmsVerifications.AddAsync(new()
		{
			Phone = request.Phone,
			Code = code,
			ExpireDate = DateTime.UtcNow.AddMinutes(20)
		});

		await dbContext.SaveChangesAsync();

		return new();
	}

	public override async Task<Empty> ValidateVerificationCode(ValidateVerificationCodeRequest request, ServerCallContext context)
	{
		bool codeValid = ushort.TryParse(request.Code, out ushort code);
		bool phoneValid = request.Phone.IsPersianPhoneValid();

		if(!codeValid || request.Code.Length != 4)
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Code\" is not valid"));
		}

		if(!phoneValid)
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Phone\" is not valid"));
		}

		SmsVerification verification = await dbContext.GetVerificationByCode(code, request.Phone) ??
									   throw new RpcException(new(StatusCode.NotFound,
																  "Parameter \"Code\" was not found"));

		if(verification.ExpireDate <= DateTime.UtcNow)
		{
			dbContext.Remove(verification);
			await dbContext.SaveChangesAsync();

			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Verification Code\" is expired"));
		}

		return new();
	}
}
