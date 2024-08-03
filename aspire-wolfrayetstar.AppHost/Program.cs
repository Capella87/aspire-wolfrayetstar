var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiApp = builder.AddProject<Projects.WolfRayetStar_Backend>("wolfrayetstar-backend");


builder.AddProject<Projects.WolfRayetStar_Front>("wolfrayetstar-front")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiApp);

builder.Build().Run();
