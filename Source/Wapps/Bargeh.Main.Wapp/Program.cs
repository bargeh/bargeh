using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Forums.Api;
using Bargeh.Main.Wapp.Client.Infrastructure;
using Bargeh.Main.Wapp.Client.Services;
using Bargeh.Main.Wapp.Components;
using Bargeh.Users.Api;
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

Uri forumsApi = new(Environment.GetEnvironmentVariable("services__forums__https__0")!);
Uri usersApi = new(Environment.GetEnvironmentVariable("services__users__https__0")!);

#region gRPC Providers

builder.Services.AddGrpcClient<ForumsProto.ForumsProtoClient>(o => o.Address = forumsApi);

builder.Services.AddGrpcClient<TopicsProto.TopicsProtoClient>(o => o.Address = forumsApi);

builder.Services.AddGrpcClient<UsersProto.UsersProtoClient>(o => o.Address = usersApi);

builder.Services.AddGrpcClient<IdentityProto.IdentityProtoClient>(o => o.Address = usersApi);

builder.Services.AddGrpcClient<SmsProto.SmsProtoClient>(o => o.Address = usersApi);

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