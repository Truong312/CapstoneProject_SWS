using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SWS.Repositories.Generic
{
    public class DapperRepository : IDapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Missing connection string MyReadOnlyDb");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<T>(sql, param, transaction);
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using var conn = CreateConnection();
            return await conn.ExecuteAsync(sql, param, transaction);
        }
    }
}