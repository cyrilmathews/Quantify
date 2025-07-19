using Dapper;
using Quantify.Estimates.Core.Entities;
using Quantify.Estimates.Core.Interfaces.Data;
using Quantify.Estimates.Core.Interfaces.Repositories;
using System.Data;

namespace Quantify.Estimates.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ClientRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT Id, Code, Name, SourceVersion, ReplicatedOn
                            FROM [Jobs].[Client]
                            WHERE Id = @Id";

                return await dbConnection.QuerySingleOrDefaultAsync<Client>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT Id, Code, Name, SourceVersion, ReplicatedOn
                            FROM [Jobs].[Client]";

                return await dbConnection.QueryAsync<Client>(sql);
            }
        }

        public async Task<int> AddAsync(Client client)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO [Jobs].[Client] (Id, Code, Name, SourceVersion)
                            VALUES (@Id, @Code, @Name, @SourceVersion);";

                var rowsAffected = await dbConnection.ExecuteAsync(sql, client);

                return rowsAffected > 0 ? client.Id : 0;
            }
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE [Jobs].[Client]
                            SET Code = @Code,
                                Name = @Name,
                                SourceVersion = @SourceVersion
                            WHERE Id = @Id AND @SourceVersion > SourceVersion;";

                var rowsAffected = await dbConnection.ExecuteAsync(sql, client);

                return rowsAffected > 0;
            }
        }
    }
}
