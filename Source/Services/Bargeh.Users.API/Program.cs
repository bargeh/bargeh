using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenLocalhost(11837, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
	options.ListenLocalhost(5001, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
});

builder.AddServiceDefaults ();

//builder.Services.AddDbContext<UsersContext> (options =>
//	options.UseMySQL (Environment.GetEnvironmentVariable ("FORUM_CONNECTION_STRING")));

builder.AddNpgsqlDbContext<UsersContext> ("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

builder.Services.AddGrpc ();

builder.Services.AddGrpcReflection ();

builder.Services.AddCors (options =>
{
	options.AddDefaultPolicy (policyBuilder =>
	{
		policyBuilder.AllowAnyOrigin ()
			.AllowAnyHeader ()
			.AllowAnyMethod ()
			.Build ();
	});
});

WebApplication app = builder.Build ();

app.UseCors ();

app.UseGrpcWeb ();

app.MapGrpcService<UsersService> ().EnableGrpcWeb ();

await UsersDbInitializer.InitializeDbAsync
	(app.Services.CreateScope ().ServiceProvider.GetRequiredService<UsersContext> (), app.Logger);

if (app.Environment.IsDevelopment ())
{
	app.MapGrpcReflectionService ();
}

app.MapGet ("/", () => "It works!");

app.Run ();