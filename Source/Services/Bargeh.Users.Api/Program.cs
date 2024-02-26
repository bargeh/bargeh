using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

//builder.Services.AddDbContext<UsersContext> (options =>
//	options.UseMySQL (Environment.GetEnvironmentVariable ("FORUM_CONNECTION_STRING")));

builder.AddNpgsqlDbContext<UsersDbContext> ("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

builder.Services.AddGrpc ();

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

builder.Services.AddGrpcReflection ();

WebApplication app = builder.Build ();

app.UseCors ();

app.UseGrpcWeb ();

app.MapGrpcService<UsersService> ().EnableGrpcWeb ();

await UsersDbInitializer.InitializeDbAsync
	(app.Services.CreateScope ().ServiceProvider.GetRequiredService<UsersDbContext> (), app.Logger);

if (app.Environment.IsDevelopment ())
{
	app.MapGrpcReflectionService ();
}

app.Run ();