using Bargeh.Main.Wapp.Client.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Identity.Api;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault (args);

builder.Services.AddScoped<LocalStorageService> ();
builder.Services.AddScoped<IdentityProviderService> ();

//builder.Services.AddGrpcClient<IdentityProto.IdentityProtoClient> (o =>
//	o.Address = new ("http://localhost:5201"));

builder.Services.AddScoped (_ =>
{
	HttpClient httpClient = new (new GrpcWebHandler (GrpcWebMode.GrpcWeb, new HttpClientHandler ()))
	{
		BaseAddress = new ("https://localhost:5201")
	};

	return GrpcChannel.ForAddress (httpClient.BaseAddress, new() { HttpClient = httpClient });
});

builder.Services.AddScoped<IdentityProto.IdentityProtoClient> ();

await builder.Build ().RunAsync ();

// FROMHERE: Fix error here :(