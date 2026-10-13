using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Endpoints.Roles; // Para buscar los permisos del rol
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Usuarios;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class IniciarSesionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/usuarios/login", Manejador)
           .WithName("IniciarSesion")
           .WithTags("Usuarios")
           .WithSummary("Inicia sesión en el sistema");
    }

    private static IResult Manejador(LoginRequest request)
    {
       
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Contrasena))
        {
            Result<UsuarioResponse> validationResult = Error.Validation(
                "Usuario.Validacion",
                "El usuario y la contraseña son obligatorios.");

            return validationResult.ToHttpResult();
        }

       
        var usuario = UsuarioDataStore.UsuariosDb
            .FirstOrDefault(u => u.VerificarCredenciales(request.Usuario, request.Contrasena));

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

        
        var rolAsociado = RolDataStore.RolesDb
            .FirstOrDefault(r => r.Nombre.Equals(usuario.Rol, StringComparison.OrdinalIgnoreCase));

        var permisos = rolAsociado?.Permisos.ToList() ?? new List<string>();

      
        var response = new UsuarioResponse(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            "*****", 
            usuario.Rol,
            usuario.Estado
        );

        return Result.Success(response).ToHttpResult();
    }
}