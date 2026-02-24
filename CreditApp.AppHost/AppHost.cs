var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.CreditApp_Api>("api")
       .WithReference(redis);

builder.Build().Run();