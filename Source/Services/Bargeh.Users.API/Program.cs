using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Services;
using Microsoft.EntityFrameworkCore;

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

app.MapGrpcService<UserService> ();

await UsersDbInitializer.InitializeDbAsync (app.Services.CreateScope (), app.Logger);

app.Run ();