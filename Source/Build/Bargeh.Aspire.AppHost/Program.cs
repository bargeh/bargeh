IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// PRODUCTION: Use a dedicated Db for each service

IResourceBuilder<PostgresContainerResource> postgres = builder
	.AddPostgresContainer("postgres", 5432, "5");

IResourceBuilder<ProjectResource> usersApi =
	builder.AddProject<Projects.Bargeh_Users_Api>("users")
		   .WithReference(postgres);

IResourceBuilder<ProjectResource> smsApi =
	builder.AddProject<Projects.Bargeh_Sms_Api>("sms")
		   .WithReference(postgres);

IResourceBuilder<ProjectResource> identityApi =
	builder.AddProject<Projects.Bargeh_Identity_Api>("identity")
		   .WithReference(usersApi)
		   .WithReference(postgres);


builder.AddProject<Projects.Bargeh_Main_Wapp>("wapp")
	   //.WithReference (sqlServer)
	   .WithReference(usersApi)
	   .WithReference(smsApi)
	   .WithReference(identityApi)
	   .WithLaunchProfile("https");


builder.Build().Run();