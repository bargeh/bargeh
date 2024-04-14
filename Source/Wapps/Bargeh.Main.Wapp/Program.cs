using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Main.Wapp.Client.Infrastructure;
using Bargeh.Main.Wapp.Client.Services;
using Bargeh.Main.Wapp.Components;
using Forums.Api;
using Grpc.Net.Client;
using Identity.Api;
using Sms.Api;
using Topics.Api;
using Users.Api;
using _Imports = Bargeh.Main.Wapp.Client._Imports;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

#region Builder

builder.Services.AddRazorComponents()
	   .AddInteractiveServerComponents()
	   .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<NotFoundListener>();

#endregion

#region gRPC Providers

builder.Services.AddSingleton(_ => new IdentityProto.IdentityProtoClient(GrpcChannel.ForAddress("http://localhost")));

builder.Services.AddSingleton(_ => new UsersProto.UsersProtoClient(GrpcChannel.ForAddress("http://localhost")));

builder.Services.AddSingleton(_ => new SmsProto.SmsProtoClient(GrpcChannel.ForAddress("http://localhost")));

builder.Services.AddGrpcClient<ForumsProto.ForumsProtoClient>(o => o.Address = new("https://localhost:5301"));

																	   builder.Services.AddGrpcClient<TopicsProto.TopicsProtoClient>(o => o.Address = new("https://localhost:7178"));

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