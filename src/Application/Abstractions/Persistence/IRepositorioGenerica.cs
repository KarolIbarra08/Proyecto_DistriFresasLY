using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Abstractions.Persistence;

/// <summary>
/// Contrato genérico de CRUD compartido por los repositorios.
/// Centraliza las operaciones que se repiten en todas las entidades:
/// consultar por identificador, consultar todas, registrar, actualizar y eliminar.
/// Cada repositorio concreto hereda este contrato y declara únicamente
/// sus métodos propios, sin duplicar el CRUD básico.
/// </summary>
/// <typeparam name="T">Entidad de dominio que administra el repositorio.</typeparam>
public interface IRepositorioGenerica<T> where T : Entity
{
    /// <summary>
    /// Obtiene una entidad mediante su identificador.
    /// </summary>
    Task<T?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las entidades registradas.
    /// </summary>
    Task<IReadOnlyList<T>> ObtenerTodosAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra una nueva entidad y devuelve su identificador.
    /// </summary>
    Task<int> AgregarAsync(
        T entidad,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza la información de una entidad existente.
    /// </summary>
    Task ActualizarAsync(
        T entidad,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una entidad mediante su identificador.
    /// </summary>
    Task EliminarAsync(
        int id,
        CancellationToken cancellationToken = default);
}
