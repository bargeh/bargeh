using Grpc.Core;
using Sms.Api;

namespace Bargeh.Sms.Api.Services;

public class SmsService (ILogger<SmsService> logger) : SmsProto.SmsProtoBase
{
	public override async Task<VoidOperationReply> SendVerification (SendVerificationRequest request,
																		ServerCallContext context)
	{
		uint code = (uint)Random.Shared.Next (1000, 9999);

		// PRODUCTION: Add real sms api
		await Task.Run (() => { logger.LogInformation ("The verification code {string} sent to {string}", code, request.Phone); });

		return new ();
	}
}