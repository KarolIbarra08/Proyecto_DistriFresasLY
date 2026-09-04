using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class IniciarSesionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/usuarios/login", Manejador)
           .WithName("IniciarSesion")
           .WithTags("Usuarios")
           .WithSummary("Inicia sesion en el sistema");
    }

    private static IResult Manejador(LoginRequest request)
    {
        var usuario = UsuarioDataStore.UsuariosDb
            .FirstOrDefault(u => u.Usuario.Equals(request.Usuario, StringComparison.OrdinalIgnoreCase) 
                              && u.Contrasena == request.Contrasena);

        if (usuario is null)
        {
            Result<UsuarioResponse> errorResult = Error.NotFound(
                "Usuario.CredencialesInvalidas", 
                "Usuario o contraseña incorrectos.");

            return errorResult.ToHttpResult();
        }

        if (!usuario.Estado)
        {
            Result<UsuarioResponse> errorResult = Error.Validation(
                "Usuario.Inactivo", 
                "El usuario se encuentra inactivo.");

            return errorResult.ToHttpResult();
        }

        var response = new UsuarioResponse(
            usuario.Id, 
            usuario.Nombre, 
            usuario.Apellido, 
            usuario.Usuario, 
            usuario.Contrasena, 
            usuario.Rol, 
            usuario.Estado);

        return Result.Success(response).ToHttpResult();
    }
}
          