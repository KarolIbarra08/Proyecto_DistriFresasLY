using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class EliminarUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/usuarios/{id:int}", Manejador)
           .WithName("EliminarUsuario")
           .WithTags("Usuarios")
           .WithSummary("Elimina un usuario del sistema por su ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IUsuarioRepository usuarioRepository,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            Result<string> errorResult = Error.NotFound(
                "Usuario.NotFound",
                $"No se encontró un usuario con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        await usuarioRepository.EliminarAsync(id, cancellationToken);

        Result<string> successResult = Result.Success($"Usuario con Id {id} eliminado correctamente.");
        return successResult.ToHttpResult();
    }
}