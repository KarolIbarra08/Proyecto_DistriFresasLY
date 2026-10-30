using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

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

    private static async Task<IResult> Manejador(
        LoginRequest request,
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Contrasena))
        {
            return Result.Failure<UsuarioResponse>(
                Error.Validation(
                    "Usuario.Validacion",
                    "El usuario y la contraseña son obligatorios."))
                .ToHttpResult();
        }

        var usuario = await usuarioRepository.ObtenerPorNombreUsuarioAsync(request.Usuario, cancellationToken);

        if (usuario is null || !usuario.VerificarCredenciales(request.Usuario, request.Contrasena))
        {
            return Result.Failure<UsuarioResponse>(
                Error.NotFound(
                    "Usuario.CredencialesInvalidas",
                    "Usuario o contraseña incorrectos."))
                .ToHttpResult();
        }

        if (!usuario.Estado)
        {
            return Result.Failure<UsuarioResponse>(
                Error.Validation(
                    "Usuario.Inactivo",
                    "El usuario se encuentra inactivo."))
                .ToHttpResult();
        }

        var rolAsociado = await rolRepository.ObtenerPorNombreAsync(usuario.Rol, cancellationToken);
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