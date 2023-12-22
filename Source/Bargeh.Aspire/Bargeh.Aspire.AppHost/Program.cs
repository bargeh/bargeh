IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder (args);

IResourceBuilder<ProjectResource> usersApi = builder
	.AddProject<Projects.Bargeh_Users_API> ("usersapi");

IResourceBuilder<ProjectResource> smsApi = builder.AddProject<Projects.Bargeh_SMS_API> ("smsapi");

//var sqlServer = builder.AddSqlServerContainer ("sqlserver");

builder.AddProject<Projects.Bargeh_Main_Wapp> ("bargehmainwapp")
	//.WithReference (sqlServer)
	.WithReference (usersApi)
	.WithReference (smsApi)
	.WithLaunchProfile ("https");

builder.AddProject<Projects.Bargeh_Identity_API>("identityapi")
	.WithReference(usersApi);

builder.Build ().Run ();