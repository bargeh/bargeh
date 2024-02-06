using Bargeh.Main.Wapp.Client.Services;
using Grpc.Net.Client.Web;
using Identity.Api;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault (args);

builder.Services.AddScoped<LocalStorageService> ();
builder.Services.AddScoped<IdentityProviderService> ();

GrpcWebHandler httpMessageHandler = new (GrpcWebMode.GrpcWeb, new HttpClientHandler ());

builder.Services.AddGrpcClient<IdentityProto.IdentityProtoClient> (o =>
{
	o.Address = new ("http://localhost:5201");
}).ConfigurePrimaryHttpMessageHandler (() => httpMessageHandler);

await builder.Build ().RunAsync ();