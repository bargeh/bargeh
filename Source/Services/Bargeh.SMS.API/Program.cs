using Bargeh.Aspire.ServiceDefaults;
using Bargeh.SMS.API.Services;

var builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

builder.Services.AddGrpc ();

var app = builder.Build ();

app.MapGrpcService<SmsService> ();

app.Run ();