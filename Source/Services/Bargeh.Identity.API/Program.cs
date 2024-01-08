using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Identity.Api.Infrastructure;
using Bargeh.Identity.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

// Add services to the container.
builder.Services.AddGrpc ();

builder.AddNpgsqlDbContext<IdentityDbContext> ("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

WebApplication app = builder.Build ();

app.MapDefaultEndpoints ();

// Configure the HTTP request pipeline.
app.MapGrpcService<IdentityService> ();

await IdentityDbInitializer.InitializeDbAsync(app.Services.CreateScope(), app.Logger);

app.Run ();
