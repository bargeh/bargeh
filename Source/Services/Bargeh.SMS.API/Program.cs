using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Sms.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

builder.Services.AddGrpc ();

WebApplication app = builder.Build ();

app.MapGrpcService<SmsService> ();

app.Run ();