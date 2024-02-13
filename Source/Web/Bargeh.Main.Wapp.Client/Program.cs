using Bargeh.Main.Wapp.Client.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Identity.Api;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sms.Api;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault (args);

builder.Services.AddScoped<LocalStorageService> ();
builder.Services.AddScoped<IdentityProviderService> ();

builder.Services.AddSingleton (_ =>
{
	HttpClient httpClient = new (new GrpcWebHandler (GrpcWebMode.GrpcWeb, new HttpClientHandler ()))
	{
		BaseAddress = new ("https://localhost:5201")
	};

	GrpcChannel channel = GrpcChannel.ForAddress (httpClient.BaseAddress, new () { HttpClient = httpClient });

	return new IdentityProto.IdentityProtoClient (channel);
});


builder.Services.AddSingleton (_ =>
{
	HttpClient httpClient = new (new GrpcWebHandler (GrpcWebMode.GrpcWeb, new HttpClientHandler ()))
	{
		BaseAddress = new ("https://localhost:5244")
	};

	GrpcChannel channel = GrpcChannel.ForAddress (httpClient.BaseAddress, new () { HttpClient = httpClient });

	return new SmsProto.SmsProtoClient (channel);
});

await builder.Build ().RunAsync ();
