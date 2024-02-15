using Bargeh.Main.Wapp.SharedServices.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Identity.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bargeh.Main.Wapp.SharedServices;

public static class Extensions
{
	public static IHostApplicationBuilder AddSharedServices (this IHostApplicationBuilder builder)
	{
		builder.Services.AddScoped<LocalStorageService>();

		builder.Services.AddSingleton(_ =>
		{
			HttpClient httpClient = new(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
			{
				BaseAddress = new("https://localhost:5201")
			};

			GrpcChannel channel = GrpcChannel.ForAddress(httpClient.BaseAddress, new() { HttpClient = httpClient });

			return new IdentityProto.IdentityProtoClient(channel);
		});


		builder.Services.AddSingleton(_ =>
		{
			HttpClient httpClient = new(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
			{
				BaseAddress = new("https://localhost:5244")
			};

			GrpcChannel channel = GrpcChannel.ForAddress(httpClient.BaseAddress, new() { HttpClient = httpClient });

			return new SmsProto.SmsProtoClient(channel);
		});

		return builder;
	}
}
