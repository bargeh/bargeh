using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Forums.Api.Services;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGrpcService<GreeterService>().EnableGrpcWeb();
app.MapGrpcReflectionService();

await ForumsDbInitializer
	.InitializeDbAsync(app.Services.CreateScope().ServiceProvider.GetRequiredService<ForumsDbContext>(),
					   app.Logger);

app.Run();