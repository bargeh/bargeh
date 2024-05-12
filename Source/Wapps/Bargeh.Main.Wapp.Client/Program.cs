using Bargeh.Forums.Api;
using Bargeh.Main.Wapp.Client.Infrastructure;
using Bargeh.Main.Wapp.Client.Services;
using Bargeh.Users.Api;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<NotFoundListener>();

builder.Services.AddSingleton(_ =>
{
	HttpClient httpClient = new(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
	{
		BaseAddress = new("https://localhost:5501")
	};

	GrpcChannel channel = GrpcChannel.ForAddress(httpClient.BaseAddress, new() { HttpClient = httpClient });

	return new IdentityProto.IdentityProtoClient(channel);
});

builder.Services.AddSingleton(_ =>
{
	HttpClient httpClient = new(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
	{
		BaseAddress = new("https://localhost:5501")
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
		BaseAddress = new("https://localhost:5301")
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