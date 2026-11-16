using DistriFresasLY.Domain.Entities.Usuarios;
namespace DistriFresasLY.Application.Abstractions.Persistence;

public interface IUsuarioRepository : IRepositorioGenerica<Usuario>
{

    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario,CancellationToken cancellationToken = default);
}
