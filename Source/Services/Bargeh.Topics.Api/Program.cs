using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Topics.Api.Infrastructure;
using Bargeh.Topics.Api.Services;
using Forums.Api;
using Users.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.AddNpgsqlDbContext<TopicsDbContext>("postgres");

builder.Services.AddGrpcClient<ForumsProto.ForumsProtoClient>(options =>
{
	options.Address = new("https://localhost:5301");
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
					 .AllowAnyMethod();
	});
});


WebApplication app = builder.Build();

app.UseCors();

app.UseGrpcWeb();
app.MapGrpcService<TopicsService>().EnableGrpcWeb();
app.MapGrpcReflectionService();

app.MapDefaultEndpoints();

await TopicsDbInitializer.InitializeDbAsync(app.Services.CreateScope().ServiceProvider
											   .GetRequiredService<TopicsDbContext>(), app.Logger);

app.Run();