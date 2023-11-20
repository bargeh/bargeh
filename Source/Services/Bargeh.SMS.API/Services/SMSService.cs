using Grpc.Core;

namespace SMS.API.Services;

public class SmsService : SMSProto.SMSProtoBase
{
	private readonly ILogger<SmsService> _logger;

	public SmsService (ILogger<SmsService> logger)
	{
		_logger = logger;
	}

	public override async Task<SendVerificationReply> SendVerification (SendVerificationRequest request,
	                                                                    ServerCallContext context)
	{
		var code = (uint)Random.Shared.Next (1000, 9999);

		// TODO: Add real sms api
		await Task.Run (() => { _logger.LogInformation ($"The verification code {code} sent to {request.Phone}"); });

		return new SendVerificationReply
		{
			Code = code.ToString()
		};
	}
}