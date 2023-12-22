using Bargeh.Aspire.ServiceDefaults;
using Bargeh.SMS.API.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

builder.Services.AddGrpc ();

WebApplication app = builder.Build ();

app.MapGrpcService<SmsService> ();

app.Run ();