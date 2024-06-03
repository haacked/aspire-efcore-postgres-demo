var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AspireDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints();

builder.Build().Run();
