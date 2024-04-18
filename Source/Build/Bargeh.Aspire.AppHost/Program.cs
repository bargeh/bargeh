using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

/*IResourceBuilder<ParameterResource> username = builder.AddParameter("dbUsername");
IResourceBuilder<ParameterResource> password = builder.AddParameter("dbPassword");*/

IResourceBuilder<PostgresServerResource> postgres =
	builder.AddPostgres("postgres", port: 5432);

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
	builder.AddProject<Bargeh_Topics_Api>("topics", launchProfileName: "https")
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