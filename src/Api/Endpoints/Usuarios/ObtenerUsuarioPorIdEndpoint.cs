using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class ObtenerUsuarioPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/usuarios/{id:int}", Manejador)
           .WithName("ObtenerUsuarioPorId")
           .WithTags("Usuarios")
           .WithSummary("Obtiene los detalles de un usuario especifico por su ID");
    }

    private static IResult Manejador(int id)
    {
        var usuario = UsuarioDataStore.UsuariosDb.FirstOrDefault(u => u.Id == id);
        if (usuario is null)
        {
            Result<UsuarioResponse> errorResult = Error.NotFound(
                "Usuario.NotFound",
                $"No se encontro un usuario con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var response = new UsuarioResponse(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.Usuario,
            usuario.Contrasena,
            usuario.Rol,
            usuario.Estado
        );

        return Result.Success(response).ToHttpResult();
    }
}