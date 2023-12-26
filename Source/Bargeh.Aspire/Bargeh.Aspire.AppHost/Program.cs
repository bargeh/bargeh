IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder (args);

IResourceBuilder<PostgresContainerResource> postgres = builder.AddPostgresContainer ("postgres", 5432);

IResourceBuilder<ProjectResource> usersApi = builder
	.AddProject<Projects.Bargeh_Users_API> ("usersapi")
	.WithReference (postgres);

IResourceBuilder<ProjectResource> smsApi = builder.AddProject<Projects.Bargeh_SMS_API> ("smsapi");

//var sqlServer = builder.AddSqlServerContainer ("sqlserver");

builder.AddProject<Projects.Bargeh_Main_Wapp> ("bargehmainwapp")
	//.WithReference (sqlServer)
	.WithReference (usersApi)
	.WithReference (smsApi)
	.WithLaunchProfile ("https");

builder.AddProject<Projects.Bargeh_Identity_API> ("identityapi")
	.WithReference (usersApi)
	.WithReference (postgres);

builder.Build ().Run ();