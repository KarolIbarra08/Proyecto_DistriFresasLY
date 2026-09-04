using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador(int id)
    {
        var index = UsuarioDataStore.UsuariosDb.FindIndex(u => u.Id == id);
        if (index == -1)
        {
            Result<bool> errorResult = Error.NotFound(
                "Usuario.NotFound",
                $"No se encontro un usuario con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        UsuarioDataStore.UsuariosDb.RemoveAt(index);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}