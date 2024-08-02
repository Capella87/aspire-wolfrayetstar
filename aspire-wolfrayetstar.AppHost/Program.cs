var builder = DistributedApplication.CreateBuilder(args);

var apiApp = builder.AddProject<Projects.WolfRayetStar_Backend>("wolfrayetstar-backend");


builder.AddProject<Projects.WolfRayetStar_Front>("wolfrayetstar-front")
    .WithExternalHttpEndpoints()
    .WithReference(apiApp);

builder.Build().Run();
