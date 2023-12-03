var builder = DistributedApplication.CreateBuilder(args);

var usersApi = builder.AddProject<Projects.Bargeh_Users_API> ("usersapi");

var smsApi = builder.AddProject<Projects.Bargeh_SMS_API> ("smsapi");

//var sqlServer = builder.AddSqlServerContainer ("sqlserver");

builder.AddProject<Projects.Bargeh_Main_Wapp> ("bargehmainwapp")
	//.WithReference (sqlServer)
	.WithReference (usersApi)
	.WithReference (smsApi)
    .WithLaunchProfile ("https");

builder.Build().Run();