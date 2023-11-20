using Bargeh.Main.Wapp.Components;

var builder = WebApplication.CreateBuilder (args);

builder.Services.AddRazorComponents ()
    .AddInteractiveServerComponents ()
    .AddInteractiveWebAssemblyComponents ();

builder.Services.AddControllers ();

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
