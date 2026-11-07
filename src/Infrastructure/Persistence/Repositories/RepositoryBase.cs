using DistriFresasLY.Application.Abstractions;
using DistriFresasLY.Infrastructure.Persistence;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

/// <summary>
/// Clase base de los repositorios Dapper.
/// Centraliza los elementos compartidos por los repositorios:
/// la fábrica de conexiones y los datos de auditoría.
/// No implementa operaciones CRUD genéricas, porque cada repositorio
/// utiliza consultas SQL específicas según las necesidades del negocio.
/// </summary>
public abstract class RepositoryBase
{
    private readonly TimeProvider _reloj;
    private readonly IUsuarioActual _usuarioActual;

    protected RepositoryBase(
        IDbConnectionFactory connectionFactory,
        TimeProvider reloj,
        IUsuarioActual usuarioActual)
    {
        ConnectionFactory = connectionFactory;
        _reloj = reloj;
        _usuarioActual = usuarioActual;
    }

    /// <summary>
    /// Fábrica utilizada para crear conexiones a la base de datos.
    /// </summary>
    protected IDbConnectionFactory ConnectionFactory { get; }

    /// <summary>
    /// Fecha y hora actual en UTC.
    /// Se utiliza para registrar fechas de creación y modificación.
    /// </summary>
    protected DateTime AhoraUtc => _reloj.GetUtcNow().UtcDateTime;

    /// <summary>
    /// Usuario que realiza la operación.
    /// Se utiliza para registrar quién crea o modifica los registros.
    /// </summary>
    protected string Usuario => _usuarioActual.Nombre;
}