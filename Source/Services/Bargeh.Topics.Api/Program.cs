using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Topics.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();

WebApplication app = builder.Build();

app.MapGrpcService<TopicsService>();

app.MapDefaultEndpoints();

app.Run();