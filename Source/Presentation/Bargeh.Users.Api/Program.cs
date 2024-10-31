using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Users.Api.Infrastructure;
using Bargeh.Users.Api.Services;
using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//builder.Services.AddDbContext<UsersContext> (options =>
//	options.UseMySQL (Environment.GetEnvironmentVariable ("FORUM_CONNECTION_STRING")));

builder.AddNpgsqlDbContext<UsersDbContext>("postgres");

builder.Services.AddGrpc();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policyBuilder =>
	{
		policyBuilder.AllowAnyOrigin()
					 .AllowAnyHeader()
					 .AllowAnyMethod();
	});
});

builder.Services.AddGrpcReflection();

// Add MassTransit services
builder.Services.AddMassTransit(x =>
{
	x.UsingRabbitMq((context, cfg) =>
	{
		cfg.Host("rabbitmq", "/", h =>
		{
			h.Username("guest");
			h.Password("guest");
		});
	});
});

WebApplication app = builder.Build();

app.UseCors();

app.Use((context, next) =>
{
	context.Response.Headers.AccessControlAllowOrigin = "*";
	context.Response.Headers.AccessControlExposeHeaders = "*";
	return next.Invoke();
});

app.UseGrpcWeb();

app.MapGrpcService<UsersService>().EnableGrpcWeb();
app.MapGrpcService<IdentityService>().EnableGrpcWeb();
app.MapGrpcService<SmsService>().EnableGrpcWeb();

UsersDbContext dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<UsersDbContext>();

await UsersDbInitializer
	.InitializeDbAsync(dbContext, app.Logger);

if(app.Environment.IsDevelopment())
{
	app.MapGrpcReflectionService();
}

app.Run();
