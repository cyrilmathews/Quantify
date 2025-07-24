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
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddSingleton<IDbConnectionFactory>(sp => new SqlConnectionFactory(connectionString));

        #region Repositories
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IJobRepository, JobRepository>();
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
        #endregion

        #region CQRS
        builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        #region Queries
        builder.Services.AddTransient<IQueryHandler<GetClientQuery, Client>, GetClientQueryHandler>();
        builder.Services.AddTransient<IQueryHandler<GetAllClientsQuery, IEnumerable<Client>>, GetAllClientsQueryHandler>();

        builder.Services.AddTransient<IQueryHandler<GetJobQuery, Job?>, GetJobQueryHandler>();
        builder.Services.AddTransient<IQueryHandler<GetAllJobsQuery, IEnumerable<Job>>, GetAllJobsQueryHandler>();
        builder.Services.AddTransient<IQueryHandler<GetJobsByClientIdQuery, IEnumerable<Job>>, GetJobsByClientIdQueryHandler>();
        #endregion

        #region Commands
        builder.Services.AddTransient<ICommandHandler<CreateClientCommand, Client>, CreateClientCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<UpdateClientCommand, bool>, UpdateClientCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<DeleteClientCommand, bool>, DeleteClientCommandHandler>();

        builder.Services.AddTransient<ICommandHandler<CreateJobCommand, int>, CreateJobCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<UpdateJobCommand, bool>, UpdateJobCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<DeleteJobCommand, bool>, DeleteJobCommandHandler>();
        
        builder.Services.AddTransient<ICommandHandler<AddOutboxEventCommand, bool>, AddOutboxEventCommandHandler>();
        #endregion

        #endregion

        AddCors(builder.Services);

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors("Development");

        app.MapControllers();

        app.Run();
    }

    private static void AddCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Development", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }
}
