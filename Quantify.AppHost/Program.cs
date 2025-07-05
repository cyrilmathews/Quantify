var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Quantify_Jobs_Api>(name: "quantify-jobs-api");

builder.Build().Run();
