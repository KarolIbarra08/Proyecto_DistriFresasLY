using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class ObtenerUsuariosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/usuarios", Manejador)
           .WithName("ObtenerUsuarios")
           .WithTags("Usuarios")
           .WithSummary("Obtiene la lista de todos los usuarios registrados");
    }

    private static async Task<IResult> Manejador(
        IUsuarioRepository usuarioRepository,
        CancellationToken cancellationToken)
    {
        var usuarios = await usuarioRepository.ObtenerTodosAsync(cancellationToken);

        var listaUsuarios = usuarios.Select(u => new UsuarioResponse(
            u.Id,
            u.Nombre,
            u.Apellido,
            u.NombreUsuario,
            u.Contrasena,
            u.Rol,
            u.Estado
        )).ToList();

        Result<List<UsuarioResponse>> successResult = Result.Success(listaUsuarios);
        return successResult.ToHttpResult();
    }
}