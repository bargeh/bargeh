using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// PRODUCTION: Use a dedicated Db for each service

// Add a parameter to the builder
IResourceBuilder<ParameterResource> myParameter = builder.AddParameter("myPaarameterName", secret: false);
IResourceBuilder<ParameterResource> your = builder.AddParameter("myParameterName", secret: false);

IResourceBuilder<PostgresServerResource> postgres = builder
	.AddPostgres("postgres", your, myParameter, 5432)
	.WithDataVolume("bghdb");

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
		   .WithReference(usersApi)
		   .AsHttp2Service();

IResourceBuilder<ProjectResource> topicsApi =
	builder.AddProject<Bargeh_Topics_Api>("topics")
		   .WithReference(postgres)
		   .WithReference(forumsApi)
		   .WithReference(usersApi)
		   .AsHttp2Service();


builder.AddProject<Bargeh_Main_Wapp>("wapp", launchProfileName: "https")
	   //.WithReference (sqlServer)
	   .WithReference(usersApi)
	   .WithReference(smsApi)
	   .WithReference(identityApi)
	   .WithReference(forumsApi)
	   .WithReference(topicsApi);

builder.Build().Run();