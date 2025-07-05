using Dapper;
using Quantify.Jobs.Core.Entities;
using Quantify.Jobs.Core.Interfaces.Data;
using Quantify.Jobs.Core.Interfaces.Repositories;
using System.Data;

namespace Quantify.Jobs.Infrastructure.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public JobRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        j.Id, j.ClientId, j.Code, j.Name, j.CreatedBy, j.CreatedOn, j.UpdatedBy, j.UpdatedOn, j.Version,
                        c.Id, c.Code, c.Name
                    FROM dbo.Job j
                    INNER JOIN dbo.Client c ON j.ClientId = c.Id
                    WHERE j.Id = @Id";

                var jobs = await dbConnection.QueryAsync<Job, Client, Job>(
                    sql,
                    (job, client) =>
                    {
                        job.Client = client;
                        return job;
                    },
                    new { Id = id },
                    splitOn: "Id"
                );

                return jobs.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        j.Id, j.ClientId, j.Code, j.Name, j.CreatedBy, j.CreatedOn, j.UpdatedBy, j.UpdatedOn, j.Version,
                        c.Id, c.Code, c.Name
                    FROM dbo.Job j
                    INNER JOIN dbo.Client c ON j.ClientId = c.Id";

                return await dbConnection.QueryAsync<Job, Client, Job>(
                    sql,
                    (job, client) =>
                    {
                        job.Client = client;
                        return job;
                    },
                    splitOn: "Id"
                );
            }
        }

        public async Task<IEnumerable<Job>> GetByClientIdAsync(int clientId)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"
                    SELECT 
                        j.Id, j.ClientId, j.Code, j.Name, j.CreatedBy, j.CreatedOn, j.UpdatedBy, j.UpdatedOn, j.Version,
                        c.Id, c.Code, c.Name
                    FROM dbo.Job j
                    INNER JOIN dbo.Client c ON j.ClientId = c.Id
                    WHERE j.ClientId = @ClientId";

                return await dbConnection.QueryAsync<Job, Client, Job>(
                    sql,
                    (job, client) =>
                    {
                        job.Client = client;
                        return job;
                    },
                    new { ClientId = clientId },
                    splitOn: "Id"
                );
            }
        }

        public async Task<int> AddAsync(Job job)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"INSERT INTO dbo.Job (ClientId, Code, Name, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn)
                            VALUES (@ClientId, @Code, @Name, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn);
                            SELECT CAST(SCOPE_IDENTITY() as int);";

                return await dbConnection.ExecuteScalarAsync<int>(sql, job);
            }
        }

        public async Task<bool> UpdateAsync(Job job)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"UPDATE dbo.Job
                            SET ClientId = @ClientId,
                                Code = @Code,
                                Name = @Name,
                                UpdatedBy = @UpdatedBy,
                                UpdatedOn = @UpdatedOn
                            WHERE Id = @Id AND Version = @Version;";

                var rowsAffected = await dbConnection.ExecuteAsync(sql, job);

                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (IDbConnection dbConnection = _connectionFactory.CreateConnection())
            {
                var sql = @"DELETE FROM dbo.Job 
                            WHERE Id = @Id;";

                var rowsAffected = await dbConnection.ExecuteAsync(sql, new { Id = id });

                return rowsAffected > 0;
            }
        }
    }
}