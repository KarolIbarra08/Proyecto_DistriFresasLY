using System.Data;
using Microsoft.Data.Sqlite;

namespace DistriFresasLY.Infrastructure.Persistence;

internal sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CrearConexion() => new SqliteConnection(_connectionString);
}