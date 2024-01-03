using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace TodoLibrary.DataAccess;

public class SqlDataAccess : IDataAccess
{
    private readonly IConfiguration _config;
    private readonly ILogger<SqlDataAccess> _log;

    public SqlDataAccess(IConfiguration config, ILogger<SqlDataAccess> log)
    {
        _config = config;
        _log = log;
    }

    public async Task<IEnumerable<T>?> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionStringName = "Default")
    {
        string? connectionString = _config.GetConnectionString(connectionStringName);
        if (connectionString != null)
        {
            using IDbConnection connection = new SqlConnection(connectionString);

            var rows = await connection.QueryAsync<T>(storedProcedure,
                parameters, commandType: CommandType.StoredProcedure);
            return rows;
        }
        else
        {
            _log.LogError("Unable to find connection string by name {ConnectionStringName}", connectionStringName);
            return null;
        }

    }

    public async Task<int> AddDataAsync<T>(string storedProcedure, T parameters, string idFieldName, string connectionStringName = "Default")
    {
        string? connectionString = _config.GetConnectionString(connectionStringName);
        if (connectionString != null)
        {
            using IDbConnection connection = new SqlConnection(connectionString);

            var p = new DynamicParameters();

            if (parameters != null)
            {
                foreach (var propertyName in parameters.GetType().GetProperties().Select(p => p.Name))
                {
                    var property = parameters.GetType().GetProperty(propertyName);
                    if (property != null)
                    {
                        object value = property.GetValue(parameters, null);
                        if (propertyName == idFieldName)
                        {
                            p.Add($"@{propertyName}", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        }
                        else
                        {
                            p.Add($"@{propertyName}", value);
                        }
                    }

                }

                await connection.ExecuteAsync(storedProcedure, p, commandType: CommandType.StoredProcedure);
                return p.Get<int>(idFieldName);
            }
            return -1;
        }
        else
        {
            _log.LogError("Unable to find connection string by name {ConnectionStringName}", connectionStringName);
            return -1;
        }
    }

    public async Task SaveDataAsync<T>(string storedProcedure, T parameters, string connectionStringName = "Default")
    {
        string? connectionString = _config.GetConnectionString(connectionStringName);
        if (connectionString != null)
        {
            using IDbConnection connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
        else
        {
            _log.LogError("Unable to find connection string by name {ConnectionStringName}", connectionStringName);
        }
    }
}
