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

WebApplication app = builder.Build ();

app.MapGrpcService<UsersService> ();

await UsersDbInitializer.InitializeDbAsync 
	(app.Services.CreateScope ().ServiceProvider.GetRequiredService<UsersContext> (), app.Logger);

app.Run ();