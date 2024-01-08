IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder (args);

// PRODUCTION: Use a dedicated Db for each service
IResourceBuilder<PostgresContainerResource> postgres = builder.AddPostgresContainer ("postgres", 5432, "5");

IResourceBuilder<ProjectResource> usersApi = builder
	.AddProject<Projects.Bargeh_Users_Api> ("usersapi")
	.WithReference (postgres);

IResourceBuilder<ProjectResource> smsApi = builder.AddProject<Projects.Bargeh_Sms_Api> ("smsapi");

//var sqlServer = builder.AddSqlServerContainer ("sqlserver");

builder.AddProject<Projects.Bargeh_Main_Wapp> ("bargehmainwapp")
	//.WithReference (sqlServer)
	.WithReference (usersApi)
	.WithReference (smsApi)
	.WithLaunchProfile ("https");

builder.AddProject<Projects.Bargeh_Identity_Api> ("identityapi")
	.WithReference (usersApi)
	.WithReference (postgres);

builder.Build ().Run ();