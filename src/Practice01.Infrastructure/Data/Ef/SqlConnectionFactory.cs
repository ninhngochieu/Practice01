using System.Data;
using Microsoft.Data.SqlClient;
using Practice01.Application.Common.Data;

namespace Practice01.Infrastructure.Data.Ef;

public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
{
    private readonly string _connectionString;
    private IDbConnection? _connection;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection GetConnection()
    {
        if (_connection is not null && _connection.State != ConnectionState.Closed)
        {
            return _connection;
        }

        _connection = new SqlConnection(_connectionString);
        _connection.Open();

        return _connection;
    }

    public void Dispose()
    {
        if (_connection == null)
        {
            return;
        }
        
        _connection.Close();
        _connection.Dispose();
    }
}