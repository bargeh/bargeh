using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Main.Wapp.Components;
using Bargeh.Main.Wapp.Infrastructure.GrpcProviders;
using SMS.API;
using Users.API;

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

builder.Services.AddSingleton<UsersApiGrpcProvider> ()
	.AddGrpcClient<UsersProto.UsersProtoClient> (o =>
		o.Address = builder.Configuration.GetValue<Uri> ("services:usersapi:1"));

builder.Services.AddSingleton<SmsApiGrpcProvider> ()
	.AddGrpcClient<SMSProto.SMSProtoClient> (o =>
		o.Address = builder.Configuration.GetValue<Uri> ("services:smsapi:1"));

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