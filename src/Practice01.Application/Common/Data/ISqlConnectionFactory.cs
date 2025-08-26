using System.Data;

namespace Practice01.Application.Common.Data;

public interface ISqlConnectionFactory
{
    IDbConnection GetConnection();
}