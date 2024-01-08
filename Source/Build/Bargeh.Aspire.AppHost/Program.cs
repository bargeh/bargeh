IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder (args);

// PRODUCTION: Use a dedicated Db for each service
IResourceBuilder<PostgresContainerResource> postgres = builder.AddPostgresContainer ("postgres", 5432, "5");

IResourceBuilder<ProjectResource> usersApi = builder
	.AddProject<Projects.Bargeh_Users_Api> ("users.api")
	.WithReference (postgres);

IResourceBuilder<ProjectResource> smsApi = builder.AddProject<Projects.Bargeh_Sms_Api> ("sms.api");

builder.AddProject<Projects.Bargeh_Main_Wapp> ("bargeh.main.wapp")
	//.WithReference (sqlServer)
	.WithReference (usersApi)
	.WithReference (smsApi)
	.WithLaunchProfile ("https");

builder.AddProject<Projects.Bargeh_Identity_Api> ("identity.api")
	.WithReference (usersApi)
	.WithReference (postgres);

builder.Build ().Run ();