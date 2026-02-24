var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CreditApp_Api>("creditapp-api");

builder.Build().Run();
