using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Sms.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

builder.Services.AddGrpc ();
builder.Services.AddGrpcReflection ();

WebApplication app = builder.Build ();

app.MapGrpcService<SmsService> ();

if (app.Environment.IsDevelopment ())
{
	app.MapGrpcReflectionService ();
}

app.Run ();