using Quantify.Estimates.Core.CQRS.Base;
using Quantify.Estimates.Core.CQRS.Commands.Client;
using Quantify.Estimates.Core.CQRS.Commands.Job;
using Quantify.Estimates.Core.CQRS.Commands.Outbox;
using Quantify.Estimates.Core.CQRS.Queries.Client;
using Quantify.Estimates.Core.CQRS.Queries.Job;
using Quantify.Estimates.Core.Entities;
using Quantify.Estimates.Core.Events.Client;
using Quantify.Estimates.Core.Interfaces.Data;
using Quantify.Estimates.Core.Interfaces.Repositories;
using Quantify.Estimates.Infrastructure.Data;
using Quantify.Estimates.Infrastructure.Messaging.BackgroundServices;
using Quantify.Estimates.Infrastructure.Repositories;

namespace Quantify.Jobs.Controller
{
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

            AddHostedServices(builder.Services);
            AddRepositories(builder.Services);
            AddCQRS(builder.Services);
            AddEvents(builder.Services);
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

        private static void AddHostedServices(IServiceCollection services)
        {
            //services.AddHostedService<ClientConsumerService>();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();
        }

        private static void AddCQRS(IServiceCollection services)
        {
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();

            // Queries
            services.AddTransient<IQueryHandler<GetClientQuery, Client>, GetClientQueryHandler>();
            services.AddTransient<IQueryHandler<GetAllClientsQuery, IEnumerable<Client>>, GetAllClientsQueryHandler>();

            services.AddTransient<IQueryHandler<GetJobQuery, Job?>, GetJobQueryHandler>();
            services.AddTransient<IQueryHandler<GetAllJobsQuery, IEnumerable<Job>>, GetAllJobsQueryHandler>();
            services.AddTransient<IQueryHandler<GetJobsByClientIdQuery, IEnumerable<Job>>, GetJobsByClientIdQueryHandler>();

            // Commands
            services.AddTransient<ICommandHandler<CreateClientCommand, Client>, CreateClientCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateClientCommand, bool>, UpdateClientCommandHandler>();

            services.AddTransient<ICommandHandler<CreateJobCommand, int>, CreateJobCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateJobCommand, bool>, UpdateJobCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteJobCommand, bool>, DeleteJobCommandHandler>();

            services.AddTransient<ICommandHandler<AddOutboxEventCommand, bool>, AddOutboxEventCommandHandler>();
        }

        private static void AddEvents(IServiceCollection services)
        {
            services.AddScoped<IEventDispatcher, EventDispatcher>();

            // Handlers
            services.AddTransient<IEventHandler<ClientCreatedEvent>, ClientCreatedEventHandler>();
        }
    }
}
