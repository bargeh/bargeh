IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// PRODUCTION: Use a dedicated Db for each service
IResourceBuilder<PostgresContainerResource> postgres = builder
	.AddPostgresContainer("postgres", 5432, "5");

IResourceBuilder<ProjectResource> usersApi =
	builder.AddProject("users", "../../../Source/Services/Bargeh.Users.Api/Bargeh.Users.API.csproj")
		   .WithReference(postgres);

IResourceBuilder<ProjectResource> smsApi =
	builder.AddProject("sms", "../../../Source/Services/Bargeh.Sms.Api/Bargeh.Sms.Api.csproj")
		   .WithReference(postgres);

IResourceBuilder<ProjectResource> identityApi =
	builder.AddProject("identity", "../../../Source/Services/Bargeh.Identity.Api/Bargeh.Identity.Api.csproj")
		   .WithReference(usersApi)
		   .WithReference(postgres);


builder.AddProject("wapp", "../../../Source/Web/Bargeh.Main.Wapp/Bargeh.Main.Wapp.csproj")
	   //.WithReference (sqlServer)
	   .WithReference(usersApi)
	   .WithReference(smsApi)
	   .WithReference(identityApi)
	   .WithLaunchProfile("https");


builder.Build().Run();