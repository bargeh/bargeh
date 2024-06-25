using Bargeh.Admin.Api.Services;
using Bargeh.Aspire.ServiceDefaults;
using Bargeh.Forums.Api.Infrastructure;
using Bargeh.Users.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddGrpcClient<UsersProto.UsersProtoClient>(o => o.Address = new("https://localhost:5501"));

builder.AddNpgsqlDbContext<ForumsDbContext>("postgres");

WebApplication app = builder.Build();


app.MapDefaultEndpoints();

app.UseGrpcWeb();

// PRODUCTION: Gprc reflection only for development
app.MapGrpcReflectionService();
app.MapGrpcService<AdminService>().EnableGrpcWeb();

app.Run();