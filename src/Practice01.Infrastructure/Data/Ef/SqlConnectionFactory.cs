using System.Data;
using Npgsql;
using Practice01.Application.Common.Data;

namespace Practice01.Infrastructure.Data;

public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable, IAsyncDisposable
{
    private readonly string _connectionString;
    private Npgsql.NpgsqlConnection? _connection;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        if (_connection is not null && _connection.State != ConnectionState.Closed)
        {
            return _connection;
        }

        _connection = new NpgsqlConnection(_connectionString);
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

    public async ValueTask DisposeAsync()
    {
        if (_connection == null)
        {
            return;
        }

        await _connection.CloseAsync();
        await _connection.DisposeAsync();
    }
}