using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres =
	builder.AddPostgres("postgres", port: 5432);

IResourceBuilder<ProjectResource> usersApi =
	builder.AddProject<Bargeh_Users_Api>("users")
		   .WithReference(postgres)
		   .AsHttp2Service();

IResourceBuilder<ProjectResource> forumsApi =
	builder.AddProject<Bargeh_Forums_Api>("forums")
		   .WithReference(postgres)
		   .WithReference(usersApi)
		   .AsHttp2Service();


builder.AddProject<Bargeh_Main_Wapp>("wapp", launchProfileName: "https")
	   .WithReference(usersApi)
	   .WithReference(forumsApi);

builder.Build().Run();