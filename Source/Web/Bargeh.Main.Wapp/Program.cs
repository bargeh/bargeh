using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Main.Wapp.Client.Services;
using Bargeh.Main.Wapp.Components;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Identity.Api;
using Sms.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

#region Builder

builder.Services.AddRazorComponents ()
	.AddInteractiveServerComponents ()
	.AddInteractiveWebAssemblyComponents ();

builder.Services.AddScoped<LocalStorageService> ();

#endregion

#region gRPC Providers

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

//builder.Services.AddSingleton<SmsApiGrpcProvider> ()
//	.AddGrpcClient<SmsProto.SmsProtoClient> (o =>
//		o.Address = builder.Configuration.GetValue<Uri> ("services:sms:1"));

#endregion

WebApplication app = builder.Build ();

#region Blazor

if (app.Environment.IsDevelopment ())
{
	app.UseWebAssemblyDebugging ();
}
else
{
	app.UseExceptionHandler ("/Error", createScopeForErrors: true);
	app.UseHsts ();
}

app.UseHttpsRedirection ();

app.UseStaticFiles ();
app.UseAntiforgery ();

app.MapRazorComponents<App> ()
	.AddInteractiveServerRenderMode ()
	.AddInteractiveWebAssemblyRenderMode ()
	.AddAdditionalAssemblies (typeof (Bargeh.Main.Wapp.Client._Imports).Assembly);

#endregion

app.Run ();
