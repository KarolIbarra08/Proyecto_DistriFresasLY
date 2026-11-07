using System.Data;

namespace DistriFresasLY.Infrastructure.Persistence;

/// <summary>
/// Fábrica de conexiones.
/// Centraliza la creación de conexiones a la base de datos.
/// </summary>
public interface IDbConnectionFactory
{
    IDbConnection CrearConexion();
}