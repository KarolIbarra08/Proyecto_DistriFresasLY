using System.Data;
using Microsoft.Data.Sqlite;

namespace DistriFresasLY.Infrastructure.Persistence;

/// <summary>
/// Implementación de la fábrica de conexiones para SQLite.
/// Entrega una conexión nueva para que los repositorios
/// puedan realizar sus operaciones con Dapper.
/// </summary>
internal sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CrearConexion()
    {
        return new SqliteConnection(_connectionString);
    }
}