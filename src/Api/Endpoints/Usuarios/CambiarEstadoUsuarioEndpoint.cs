using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class CambiarEstadoUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/usuarios/{id:int}/estado", Manejador)
           .WithName("CambiarEstadoUsuario")
           .WithTags("Usuarios")
           .WithSummary("Activa o desactiva un usuario en el sistema");
    }

    private static IResult Manejador(int id)
    {
        var index = UsuarioDataStore.UsuariosDb.FindIndex(u => u.Id == id);
        if (index == -1)
        {
            Result<UsuarioResponse> errorResult = Error.NotFound(
                "Usuario.NotFound", 
                $"No se encontró un usuario con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var usuarioExistente = UsuarioDataStore.UsuariosDb[index];
        var nuevoEstado = !usuarioExistente.Estado;

        var usuarioActualizado = usuarioExistente with { Estado = nuevoEstado };
        UsuarioDataStore.UsuariosDb[index] = usuarioActualizado;

        var response = new UsuarioResponse(
            usuarioActualizado.Id, 
            usuarioActualizado.Nombre, 
            usuarioActualizado.Apellido, 
            usuarioActualizado.Usuario, 
            usuarioActualizado.Contrasena, 
            usuarioActualizado.Rol, 
            usuarioActualizado.Estado);

        return Result.Success(response).ToHttpResult();
    }
}