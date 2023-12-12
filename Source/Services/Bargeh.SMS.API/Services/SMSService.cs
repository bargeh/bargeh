using Grpc.Core;
using SMS.API;

namespace Bargeh.SMS.API.Services;

public class SmsService (ILogger<SmsService> logger) : SMSProto.SMSProtoBase
{
	public override async Task<SendVerificationReply> SendVerification (SendVerificationRequest request,
																		ServerCallContext context)
	{
		var code = (uint)Random.Shared.Next (1000, 9999);

		// TODO: Add real sms api
		await Task.Run (() => { logger.LogInformation ("The verification code {0} sent to {1}", code, request.Phone); });

		return new ()
		{
			Code = code.ToString ()
		};
	}
}