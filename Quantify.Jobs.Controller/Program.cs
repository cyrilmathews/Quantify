using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.CQRS.Commands.Client;
using Quantify.Jobs.Core.CQRS.Commands.Job;
using Quantify.Jobs.Core.CQRS.Commands.Outbox;
using Quantify.Jobs.Core.CQRS.Queries.Client;
using Quantify.Jobs.Core.CQRS.Queries.Job;
using Quantify.Jobs.Core.Entities;
using Quantify.Jobs.Core.Interfaces.Data;
using Quantify.Jobs.Core.Interfaces.Repositories;
using Quantify.Jobs.Infrastructure.Data;
using Quantify.Jobs.Infrastructure.Repositories;

namespace Quantify.Jobs.Controller;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddSingleton<IDbConnectionFactory>(sp => new SqlConnectionFactory(connectionString));

        AddAuthentication(builder);
        AddAuthorization(builder);

        AddCors(builder);

        AddRepositories(builder);
        AddCQRS(builder);


        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors("Development");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    private static void AddAuthorization(WebApplicationBuilder builder)
    {
        // Add Authorization policies if you need more granular control than simple roles
        builder.Services.AddAuthorization(options =>
        {
            // Define a policy for "Products.ReadWrite" scope
            options.AddPolicy("RequiresJobsAllScope", policy =>
                policy.RequireClaim("http://schemas.microsoft.com/identity/claims/scope", builder.Configuration["AllowedScopes"]));
            // Or, if using application permissions (app roles for daemon apps), you might use:
            // policy.RequireRole("Products.ReadWrite.App");
        });
    }

    private static void AddAuthentication(WebApplicationBuilder builder)
    {
        // Configure Azure AD B2C authentication with Microsoft.Identity.Web
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
    }

    private static void AddCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Development", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }

    private static void AddRepositories(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IJobRepository, JobRepository>();
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
    }

    private static void AddCQRS(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        AddQueries(builder);
        AddCommands(builder);
    }

    private static void AddQueries(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IQueryHandler<GetClientQuery, Client>, GetClientQueryHandler>();
        builder.Services.AddTransient<IQueryHandler<GetAllClientsQuery, IEnumerable<Client>>, GetAllClientsQueryHandler>();

        builder.Services.AddTransient<IQueryHandler<GetJobQuery, Job?>, GetJobQueryHandler>();
        builder.Services.AddTransient<IQueryHandler<GetAllJobsQuery, IEnumerable<Job>>, GetAllJobsQueryHandler>();
        builder.Services.AddTransient<IQueryHandler<GetJobsByClientIdQuery, IEnumerable<Job>>, GetJobsByClientIdQueryHandler>();
    }

    private static void AddCommands(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICommandHandler<CreateClientCommand, Client>, CreateClientCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<UpdateClientCommand, bool>, UpdateClientCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<DeleteClientCommand, bool>, DeleteClientCommandHandler>();

        builder.Services.AddTransient<ICommandHandler<CreateJobCommand, int>, CreateJobCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<UpdateJobCommand, bool>, UpdateJobCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<DeleteJobCommand, bool>, DeleteJobCommandHandler>();

        builder.Services.AddTransient<ICommandHandler<AddOutboxEventCommand, bool>, AddOutboxEventCommandHandler>();
    }
}
