using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithImage("redis:alpine")
    .WithLifetime(ContainerLifetime.Session);

var api = builder.AddProject<Projects.CreditApp_Api>("api")
    .WithReference(redis);

builder.Build().Run();