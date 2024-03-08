using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Topics.Api.Infrastructure;
using Bargeh.Topics.Api.Services;
using Forums.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddNpgsqlDbContext<TopicsDbContext>("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

builder.Services.AddGrpcClient<ForumsProto.ForumsProtoClient>(options =>
{
	options.Address = new(builder.Configuration.GetValue<string>("services:forums:1")!);
});

WebApplication app = builder.Build();

app.UseGrpcWeb();
app.MapGrpcService<TopicsService>().EnableGrpcWeb();
app.MapGrpcReflectionService();

app.MapDefaultEndpoints();

app.Run();