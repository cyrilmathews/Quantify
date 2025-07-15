var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Quantify_Jobs_Api>(name: "quantify-jobs-api");

builder.AddAzureFunctionsProject<Projects.Quantify_Jobs_FunctionApp>("quantify-jobs-functionapp");

builder.Build().Run();
