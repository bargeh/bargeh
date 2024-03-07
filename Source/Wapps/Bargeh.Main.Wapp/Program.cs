using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Main.Wapp.Client.Services;
using Bargeh.Main.Wapp.Components;
using Grpc.Net.Client;
using Identity.Api;
using Sms.Api;
using Users.Api;
using _Imports = Bargeh.Main.Wapp.Client._Imports;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

#region Builder

builder.Services.AddRazorComponents()
	   .AddInteractiveServerComponents()
	   .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<LocalStorageService>();

#endregion

#region gRPC Providers

builder.Services.AddSingleton(_ => new IdentityProto.IdentityProtoClient(GrpcChannel.ForAddress("http://localhost")));

builder.Services.AddSingleton(_ => new UsersProto.UsersProtoClient(GrpcChannel.ForAddress("http://localhost")));

builder.Services.AddSingleton(_ => new SmsProto.SmsProtoClient(GrpcChannel.ForAddress("http://localhost")));

//builder.Services.AddSingleton<SmsApiGrpcProvider> ()
//	.AddGrpcClient<SmsProto.SmsProtoClient> (o =>
//		o.Address = builder.Configuration.GetValue<Uri> ("services:sms:1"));

#endregion

WebApplication app = builder.Build();

#region Blazor

if(app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error", true);
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies(typeof(_Imports).Assembly);

#endregion

app.Run();