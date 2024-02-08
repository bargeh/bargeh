using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

//builder.Services.AddDbContext<UsersContext> (options =>
//	options.UseMySQL (Environment.GetEnvironmentVariable ("FORUM_CONNECTION_STRING")));

builder.AddNpgsqlDbContext<UsersContext> ("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

builder.Services.AddGrpc ();

builder.Services.AddCors (options =>
{
	options.AddPolicy ("test", policyBuilder =>
	{
		policyBuilder.AllowAnyOrigin ()
			.AllowAnyHeader ()
			.AllowAnyMethod ()
			.Build ();
	});
});

builder.Services.AddGrpcReflection ();

WebApplication app = builder.Build ();

app.UseCors ("test");

app.MapGrpcService<UsersService> ();

await UsersDbInitializer.InitializeDbAsync
	(app.Services.CreateScope ().ServiceProvider.GetRequiredService<UsersContext> (), app.Logger);

if (app.Environment.IsDevelopment ())
{
	app.MapGrpcReflectionService ();
}

app.Run ();