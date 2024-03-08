using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// PRODUCTION: Use a dedicated Db for each service

IResourceBuilder<PostgresContainerResource> postgres = builder
	.AddPostgresContainer("postgres", 5432, "5");

IResourceBuilder<ProjectResource> usersApi =
	builder.AddProject<Bargeh_Users_Api>("users")
		   .WithReference(postgres)
		   .AsHttp2Service();

IResourceBuilder<ProjectResource> smsApi =
	builder.AddProject<Bargeh_Sms_Api>("sms")
		   .WithReference(postgres)
		   .WithReference(usersApi)
		   .AsHttp2Service();

IResourceBuilder<ProjectResource> identityApi =
	builder.AddProject<Bargeh_Identity_Api>("identity")
		   .WithReference(usersApi)
		   .WithReference(postgres)
		   .AsHttp2Service();

IResourceBuilder<ProjectResource> forumsApi =
	builder.AddProject<Bargeh_Forums_Api>("forums")
		   .WithReference(postgres)
		   .AsHttp2Service();

IResourceBuilder<ProjectResource> topicsApi =
	builder.AddProject<Bargeh_Topics_Api>("topics")
		   .WithReference(postgres)
		   .AsHttp2Service();


builder.AddProject<Bargeh_Main_Wapp>("wapp")
	   //.WithReference (sqlServer)
	   .WithReference(usersApi)
	   .WithReference(smsApi)
	   .WithReference(identityApi)
	   .WithReference(forumsApi)
	   .WithLaunchProfile("https");


builder.Build().Run();