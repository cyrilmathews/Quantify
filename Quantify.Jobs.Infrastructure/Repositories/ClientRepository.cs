using Dapper;
using Microsoft.Data.SqlClient;
using Quantify.Jobs.Core.Entities;
using Quantify.Jobs.Core.Interfaces.Data;
using Quantify.Jobs.Core.Interfaces.Repositories;
using System.Data;

namespace Quantify.Jobs.Infrastructure.Repository
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
                var sql = @"SELECT Id, Code, Name, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn, Version 
                            FROM dbo.Client 
                            WHERE Id = @Id";

                return await dbConnection.QuerySingleOrDefaultAsync<Client>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"SELECT Id, Code, Name, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn, Version 
                            FROM dbo.Client";

                return await dbConnection.QueryAsync<Client>(sql);
            }
        }

        public async Task<int> AddAsync(Client client)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO dbo.Client (Code, Name, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn)
                            VALUES (@Code, @Name, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn);
                            SELECT CAST(SCOPE_IDENTITY() as int);"; 

                return await dbConnection.ExecuteScalarAsync<int>(sql, client);
            }
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE dbo.Client
                            SET Code = @Code,
                                Name = @Name,
                                UpdatedBy = @UpdatedBy,
                                UpdatedOn = @UpdatedOn
                            WHERE Id = @Id AND Version = @Version;";

                var rowsAffected = await dbConnection.ExecuteAsync(sql, client);
                
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"DELETE FROM dbo.Client 
                            WHERE Id = @Id;";

                var rowsAffected = await dbConnection.ExecuteAsync(sql, new { Id = id });

                return rowsAffected > 0;
            }
        }
    }
}
