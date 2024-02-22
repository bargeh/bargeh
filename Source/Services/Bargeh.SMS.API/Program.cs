using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Sms.Api.Infrastructure;
using Bargeh.Sms.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

builder.Services.AddGrpc ();
builder.Services.AddGrpcReflection ();

builder.AddNpgsqlDbContext<SmsDbContext> ("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policyBuilder =>
	{
		policyBuilder.AllowAnyOrigin()
					 .AllowAnyHeader()
					 .AllowAnyMethod()
					 .WithExposedHeaders("grpc-status", "grpc-message");
	});
});


WebApplication app = builder.Build ();

app.UseCors ();

app.MapDefaultEndpoints();

app.UseGrpcWeb ();

app.MapGrpcService<SmsService> ().EnableGrpcWeb ();

if (app.Environment.IsDevelopment ())
{
	app.MapGrpcReflectionService ();
}

await SmsDbInitializer.InitializeDbAsync (app.Services.CreateScope().ServiceProvider.GetRequiredService<SmsDbContext> (), app.Logger);

app.MapGet("/", () => "I got you");

app.Run ();
