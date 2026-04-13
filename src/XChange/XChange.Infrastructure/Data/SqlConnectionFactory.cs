using Microsoft.Data.SqlClient;
using System.Data;
using XChange.Core.Interfaces;

namespace XChange.Infrastructure.Data
{
    public class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
    {
        public IDbConnection CreateConnection() => new SqlConnection(connectionString);
    }
}
