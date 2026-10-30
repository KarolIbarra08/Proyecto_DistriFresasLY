using System.Data;

namespace DistriFresasLY.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection CrearConexion();
}