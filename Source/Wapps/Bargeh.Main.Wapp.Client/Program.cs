using Bargeh.Main.Wapp.Client.Infrastructure;
using Bargeh.Main.Wapp.Client.Services;
using Forums.Api;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Identity.Api;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sms.Api;
using Topics.Api;
using Users.Api;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<NotFoundListener>();

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

builder.Services.AddSingleton(_ =>
{
	HttpClient httpClient = new(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
	{
		BaseAddress = new("https://localhost:5501")
	};

	GrpcChannel channel = GrpcChannel.ForAddress(httpClient.BaseAddress, new() { HttpClient = httpClient });

	return new UsersProto.UsersProtoClient(channel);
});

builder.Services.AddSingleton(_ =>
{
	HttpClient httpClient = new(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
	{
		BaseAddress = new("https://localhost:7178")
	};

	GrpcChannel channel = GrpcChannel.ForAddress(httpClient.BaseAddress, new() { HttpClient = httpClient });

	return new TopicsProto.TopicsProtoClient(channel);
});

builder.Services.AddSingleton(_ =>
{
	HttpClient httpClient = new(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
	{
		BaseAddress = new("https://localhost:5301")
	};

	GrpcChannel channel = GrpcChannel.ForAddress(httpClient.BaseAddress, new() { HttpClient = httpClient });

	return new ForumsProto.ForumsProtoClient(channel);
});

await builder.Build().RunAsync();