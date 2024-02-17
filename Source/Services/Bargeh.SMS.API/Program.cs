using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Sms.Api.Infrastructure;
using Bargeh.Sms.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

builder.Services.AddGrpc ();
builder.Services.AddGrpcReflection ();

// FROMHERE: Fix Db DI problem

builder.AddNpgsqlDbContext<SmsDbContext> ("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

builder.Services.AddCors (options =>
{
	options.AddDefaultPolicy (policyBuilder =>
	{
		policyBuilder.AllowAnyOrigin ()
			.AllowAnyMethod ()
			.AllowAnyHeader ();
	});
});

WebApplication app = builder.Build ();

app.UseCors ();

app.UseGrpcWeb ();

app.MapGrpcService<SmsService> ().EnableGrpcWeb ();

if (app.Environment.IsDevelopment ())
{
	app.MapGrpcReflectionService ();
}

await SmsDbInitializer.InitializeDbAsync (app.Services.GetRequiredService<SmsDbContext> (), app.Logger);

app.Run ();
