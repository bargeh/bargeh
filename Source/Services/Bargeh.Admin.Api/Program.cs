using Bargeh.Admin.Api.Services;
using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Forums.Api.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddNpgsqlDbContext<ForumsDbContext>("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseGrpcWeb();
app.MapGrpcService<AdminService>().EnableGrpcWeb();

app.Run();