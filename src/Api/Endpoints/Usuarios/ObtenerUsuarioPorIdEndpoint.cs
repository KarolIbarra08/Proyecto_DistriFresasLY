using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class ObtenerUsuarioPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/usuarios/{id:int}", Manejador)
           .WithName("ObtenerUsuarioPorId")
           .WithTags("Usuarios")
           .WithSummary("Obtiene un usuario por su ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IUsuarioRepository usuarioRepository,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            Result<UsuarioResponse> errorResult = Error.NotFound(
                "Usuario.NotFound",
                $"No se encontró un usuario con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var response = new UsuarioResponse(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            usuario.Rol,
            usuario.Estado
        );

        Result<UsuarioResponse> successResult = Result.Success(response);
        return successResult.ToHttpResult();
    }
}