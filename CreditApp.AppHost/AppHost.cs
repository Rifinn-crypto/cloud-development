using Google.Protobuf.WellKnownTypes;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithRedisCommander();

var api = builder.AddProject<Projects.CreditApp_Api>("api")
       .WithReference(redis)
       .WaitFor(redis);

builder.AddProject<Projects.Client_Wasm>("client")
     .WithReference(api)
     .WaitFor(api);

builder.Build().Run();

