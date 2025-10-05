using Transferencia.Api.Application.Abstractions;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Transferencia.Api.Infrastructure
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqliteConnection(connectionString);
        }
    }
}