var builder = DistributedApplication.CreateBuilder(args);

//redis
var cache = builder.AddRedis("cache");

//Connection DB
var dbConnection = builder.AddSqlServer("sqldata")
                       .WithDataVolume("sql-data")
                       .AddDatabase("partonairdb");

//var partonairDbLOCAL = builder.AddConnectionString("partonairdb");

var server = builder.AddProject<Projects.API_partonair_v01>("server")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(dbConnection)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithUrl("https://localhost:7540/scalar", "scalar");

var webfrontend = builder.AddViteApp("webfrontend", "../frontend.partonair-v01")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
