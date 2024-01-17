using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Identity.Api.Infrastructure;
using Bargeh.Identity.Api.Services;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Users.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

// Add services to the container.
builder.Services.AddGrpc ();

builder.AddNpgsqlDbContext<IdentityDbContext> ("postgres", settings =>
{
    settings.MaxRetryCount = 10;
});

builder.Services.AddGrpcClient<UsersProto.UsersProtoClient> (options =>
{
    options.Address = new (builder.Configuration.GetValue<string> ("services:users:1")!);
});

WebApplication app = builder.Build ();

app.MapDefaultEndpoints ();

// Configure the HTTP request pipeline.
app.MapGrpcService<IdentityService> ();

await IdentityDbInitializer.InitializeDbAsync (app.Services.CreateScope (), app.Logger);

app.Run ();