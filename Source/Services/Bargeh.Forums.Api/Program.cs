using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Forums.Api.Services;
using Bargeh.Users.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddNpgsqlDbContext<ForumsDbContext>("postgres");

Uri usersApi = new(Environment.GetEnvironmentVariable("services__users__https__0")!);

builder.Services.AddGrpcClient<UsersProto.UsersProtoClient>(options =>
{
	options.Address = usersApi;
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


WebApplication app = builder.Build();

app.UseCors();

app.MapDefaultEndpoints();

app.Use((context, next) =>
{
	context.Response.Headers.AccessControlAllowOrigin = "*";
	context.Response.Headers.AccessControlExposeHeaders = "*";
	return next.Invoke();
});

app.UseGrpcWeb();
app.MapGrpcService<ForumsService>().EnableGrpcWeb();
app.MapGrpcService<TopicsService>().EnableGrpcWeb();
app.MapGrpcReflectionService();

await ForumsDbInitializer
	.InitializeDbAsync(app.Services.CreateScope().ServiceProvider.GetRequiredService<ForumsDbContext>(),
					   app.Logger);

app.Run();