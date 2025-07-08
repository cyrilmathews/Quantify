
using Quantify.Jobs.Core.CQRS.Base;
using Quantify.Jobs.Core.CQRS.Commands.Client;
using Quantify.Jobs.Core.CQRS.Queries.Client;
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
        #endregion

        #region CQRS
        builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        #region Queries
        builder.Services.AddTransient<IQueryHandler<GetClientQuery, Client>, GetClientQueryHandler>();
        builder.Services.AddTransient<IQueryHandler<GetAllClientsQuery, IEnumerable<Client>>, GetAllClientsQueryHandler>();
        #endregion

        #region Commands
        builder.Services.AddTransient<ICommandHandler<CreateClientCommand, int>, CreateClientCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<UpdateClientCommand, bool>, UpdateClientCommandHandler>();
        builder.Services.AddTransient<ICommandHandler<DeleteClientCommand, bool>, DeleteClientCommandHandler>();
        #endregion

        #endregion

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
