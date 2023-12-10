using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Main.Wapp.Components;
using Bargeh.Main.Wapp.Infrastructure.GrpcProviders;
using Users.API;

var builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

builder.Services.AddRazorComponents ()
    .AddInteractiveServerComponents ()
    .AddInteractiveWebAssemblyComponents ();

builder.Services.AddControllers ();

builder.Services.AddHttpContextAccessor ();


builder.Services.AddSingleton<UsersApiGrpcProvider>()
	.AddGrpcClient<UsersProto.UsersProtoClient>(o =>
		o.Address = builder.Configuration.GetValue<Uri>("services:usersapi:1"));

var app = builder.Build ();

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

app.Run ();