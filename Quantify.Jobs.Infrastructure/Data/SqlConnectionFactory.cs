using Microsoft.Data.SqlClient;
using Quantify.Jobs.Core.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantify.Jobs.Infrastructure.Data
{
    /// <summary>
    /// Implements IDbConnectionFactory for SQL Server connections.
    /// </summary>
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates and returns a new SqlConnection.
        /// This method leverages ADO.NET's built-in connection pooling.
        /// </summary>
        /// <returns>A new SqlConnection instance.</returns>
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
