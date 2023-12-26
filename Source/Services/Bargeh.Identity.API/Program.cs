using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Identity.API.Infrastructure;
using Bargeh.Identity.API.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

// Add services to the container.
builder.Services.AddGrpc ();

builder.Services.AddNpgsql<IdentityDbContext>("postgres");

WebApplication app = builder.Build ();

app.MapDefaultEndpoints ();

// Configure the HTTP request pipeline.
app.MapGrpcService<IdentityService> ();

app.Run ();
