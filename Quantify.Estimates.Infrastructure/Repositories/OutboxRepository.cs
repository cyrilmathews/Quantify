using Dapper;
using Quantify.Estimates.Core.Entities;
using Quantify.Estimates.Core.Interfaces.Data;
using Quantify.Estimates.Core.Interfaces.Repositories;
using System.Data;

namespace Quantify.Estimates.Infrastructure.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OutboxRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> AddAsync(Outbox outbox)
        {
            using IDbConnection dbConnection = _connectionFactory.CreateConnection();

            const string sql = @"
                INSERT INTO [Outbox] (
                    [Id],
                    [EventType],
                    [EventData],
                    [Timestamp],
                    [IsProcessed],
                    [ProcessedDate]
                )
                VALUES (
                    @Id,
                    @EventType,
                    @EventData,
                    @Timestamp,
                    @IsProcessed,
                    @ProcessedDate
                );";

            var affectedRows = await dbConnection.ExecuteAsync(sql, new
            {
                outbox.Id,
                outbox.EventType,
                outbox.EventData,
                outbox.Timestamp,
                outbox.IsProcessed,
                outbox.ProcessedDate
            });

            return affectedRows > 0;
        }
    }
}
