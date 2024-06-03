var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireDemo_ApiService>("apiservice");

builder.AddProject<Projects.AspireDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
