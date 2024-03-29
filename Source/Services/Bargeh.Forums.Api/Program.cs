using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Forums.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddNpgsqlDbContext<ForumsDbContext>("postgres", settings =>
{
	settings.MaxRetryCount = 10;
});

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policyBuilder =>
	{
		policyBuilder.AllowAnyOrigin()
					 .AllowAnyHeader()
					 .AllowAnyMethod();
	});
});


WebApplication app = builder.Build ();

app.UseCors ();

app.MapDefaultEndpoints();

app.Use((context, next) =>
{
	context.Response.Headers.AccessControlAllowOrigin = "*";
	context.Response.Headers.AccessControlExposeHeaders = "*";
	return next.Invoke();
});

app.UseGrpcWeb();
app.MapGrpcService<ForumsService>().EnableGrpcWeb();
app.MapGrpcReflectionService();

await ForumsDbInitializer
	.InitializeDbAsync(app.Services.CreateScope().ServiceProvider.GetRequiredService<ForumsDbContext>(),
					   app.Logger);

app.Run();