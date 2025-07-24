# Quantify

Quantify is a modular .NET 9 Job costing system, featuring robust database migration, API services, and background processing. The solution leverages modern .NET features, Aspire hosting, and best practices for scalable cloud-native applications.

## Solution Structure

- **Quantify.AppHost**  
  Entry point for hosting the application using Aspire. Manages service orchestration and dependencies.

- **Quantify.Jobs.Controller**  
  ASP.NET Core Web API for managing jobs and clients.  
  - Target Framework: .NET 9  
  - Features OpenAPI support for API documentation.

- **Quantify.Jobs.FunctionApp**  
  Azure Functions project for background/outbox event publishing.

- **Quantify.Jobs.Database**  
  Console app for database migrations using DbUp.  
  - Reads configuration from `appsettings.json` and environment variables.  
  - Executes embedded SQL scripts for schema and triggers.

- **Quantify.Estimates.Api**  
  ASP.NET Core Web API for managing estimates, jobs, and clients.

- **Quantify.Estimates.Database**  
  Console app for database migrations (DbUp).  
  - Handles schema creation and audit triggers.

- **Quantify.Jobs.Core / Quantify.Estimates.Core**  
  Domain models, CQRS commands, and events for jobs and estimates.

- **Quantify.Jobs.Infrastructure / Quantify.Estimates.Infrastructure**  
  Data access, repositories, and background services.

- **Quantify.ServiceDefaults**  
  Shared service configuration and defaults.

## Key Technologies

- **.NET 9 / C# 13**
- **Aspire Hosting**
- **DbUp** for database migrations
- **Dapper** for data access
- **Azure Functions**
- **OpenAPI/Swagger** for API documentation
