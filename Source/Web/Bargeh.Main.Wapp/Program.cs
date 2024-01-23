using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Main.Wapp.Components;
using Identity.Api;
using Sms.Api;
using Users.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

#region Blazor

builder.Services.AddRazorComponents ()
	.AddInteractiveServerComponents ()
	.AddInteractiveWebAssemblyComponents ();

builder.Services.AddControllers ();

builder.Services.AddHttpContextAccessor ();


#endregion

#region gRPC Providers

builder.Services.AddGrpcClient<IdentityProto.IdentityProtoClient> (o =>
		o.Address = builder.Configuration.GetValue<Uri> ("services:identity:1"));

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

app.MapDefaultControllerRoute ();

#endregion

app.Run ();