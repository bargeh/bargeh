using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Identity.Api.Infrastructure;
using Bargeh.Identity.Api.Services;
using Users.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddNpgsqlDbContext<IdentityDbContext>("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

builder.Services.AddGrpcClient<UsersProto.UsersProtoClient>(options =>
{
	options.Address = new("https://localhost:5501");
});

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policyBuilder =>
	{
		policyBuilder.AllowAnyOrigin()
					 .AllowAnyHeader()
					 .AllowAnyMethod()
					 .Build();
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

app.MapDefaultEndpoints();

app.UseGrpcWeb();

app.MapGrpcService<IdentityService>().EnableGrpcWeb();

await IdentityDbInitializer
	.InitializeDbAsync(app.Services.CreateScope().ServiceProvider.GetRequiredService<IdentityDbContext>(),
					   app.Logger);

if(app.Environment.IsDevelopment())
{
	app.MapGrpcReflectionService();
}

app.MapGet("/", () => "wow");

app.Run();