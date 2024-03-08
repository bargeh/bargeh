using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Topics.Api.Infrastructure;
using Bargeh.Topics.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddNpgsqlDbContext<TopicsDbContext>("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

WebApplication app = builder.Build();

app.UseGrpcWeb();
app.MapGrpcService<TopicsService>().EnableGrpcWeb();
app.MapGrpcReflectionService();

app.MapDefaultEndpoints();

app.Run();