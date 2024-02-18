using Bargeh.Sms.Api.Infrastructure;
using Bargeh.Sms.Api.Models;
using Grpc.Core;
using MatinDevs.PersianPhoneNumbers;
using Sms.Api;
using static System.UInt16;

namespace Bargeh.Sms.Api.Services;

public class SmsService(SmsDbContext dbContext) : SmsProto.SmsProtoBase
{
	public override async Task<VoidOperationReply> SendVerification(SendVerificationRequest request,
																	ServerCallContext context)
	{
		ushort code = (ushort)Random.Shared.Next(1000, 9999);

		// PRODUCTION: Add a real SMS sending API contract here

		await dbContext.SmsVerifications.AddAsync(new()
		{
			Phone = request.Phone,
			Code = code,
			ExpireDate = DateTime.UtcNow.AddMinutes(5)
		});

		await dbContext.SaveChangesAsync();

		return new();
	}

	public override async Task<ValidateVerificationCodeReply> ValidateVerificationCode(
		ValidateVerificationCodeRequest request,
		ServerCallContext context)
	{
		bool codeValid = TryParse(request.Code, out ushort code);
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
																  "Parameter \"Code\" is not valid"));

		if(verification.ExpireDate >= DateTime.UtcNow)
		{
			dbContext.Remove(verification);
			await dbContext.SaveChangesAsync();

			throw new RpcException(new(StatusCode.InvalidArgument, "Parameter \"Verification Code\" is expired"));
		}

		if(code != verification.Code)
		{
			throw new RpcException(new(StatusCode.InvalidArgument, "The verification code is incorrect"));
		}


		return new();
	}
}