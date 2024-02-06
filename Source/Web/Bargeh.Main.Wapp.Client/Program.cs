using Bargeh.Main.Wapp.Client.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Identity.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault (args);

builder.Services.AddScoped<LocalStorageService> ();
builder.Services.AddScoped<IdentityProviderService> ();

builder.Services.AddGrpcClient<IdentityProto.IdentityProtoClient> (o =>
	o.Address = new ("http://localhost:5201"));

await builder.Build ().RunAsync ();