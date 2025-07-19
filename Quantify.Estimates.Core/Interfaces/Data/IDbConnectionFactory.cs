using System.Data;

namespace Quantify.Estimates.Core.Interfaces.Data
{
    /// <summary>
    /// Defines a factory for creating database connections.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates and returns a new database connection.
        /// The caller is responsible for disposing the connection.
        /// </summary>
        /// <returns>An IDbConnection instance.</returns>
        public IDbConnection CreateConnection();
    }
}
