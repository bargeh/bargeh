using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Users.API.Infrastructure;
using Bargeh.Users.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder (args);

builder.AddServiceDefaults ();

//builder.Services.AddDbContext<UsersContext> (options =>
//	options.UseMySQL (Environment.GetEnvironmentVariable ("FORUM_CONNECTION_STRING")));

builder.Services.AddDbContext<UsersContext> (options =>
	options.UseInMemoryDatabase ("test"));

builder.Services.AddGrpc ();

var app = builder.Build ();

app.MapGrpcService<UserService> ();
app.MapGet ("/",
	() =>
		"Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await UsersDbInitializer.InitializeDbAsync (app.Services.CreateScope (), app.Logger);

app.Run ();