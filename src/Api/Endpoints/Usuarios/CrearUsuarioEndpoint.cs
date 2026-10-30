using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Roles;
using DistriFresasLY.Domain.Entities.Usuarios;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class CrearUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/usuarios", Manejador)
           .WithName("CrearUsuario")
           .WithTags("Usuarios")
           .WithSummary("Crea un nuevo usuario en el sistema");
    }

    private static async Task<IResult> Manejador(
        CrearUsuarioRequest request,
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        CancellationToken cancellationToken)
    {

        var usuarioExistente = await usuarioRepository.ObtenerPorNombreUsuarioAsync(request.Usuario, cancellationToken);
        if (usuarioExistente is not null)
        {
            Result<UsuarioResponse> errorUsuarioExistente = Error.Conflict(
                "Usuario.NombreUsuarioDuplicado",
                $"El nombre de usuario '{request.Usuario}' ya está en uso.");

            return errorUsuarioExistente.ToHttpResult();
        }


        Rol? rolExistente = null;

        if (int.TryParse(request.Rol, out int idRol))
        {
            rolExistente = await rolRepository.ObtenerPorIdAsync(idRol, cancellationToken);
        }
        else
        {
            rolExistente = await rolRepository.ObtenerPorNombreAsync(request.Rol, cancellationToken);
        }

        if (rolExistente is null)
        {
            Result<UsuarioResponse> errorRolNoEncontrado = Error.NotFound(
                "Usuario.RolNoEncontrado",
                $"El rol '{request.Rol}' no existe en la base de datos.");

            return errorRolNoEncontrado.ToHttpResult();
        }


        var usuarioResult = Usuario.Create(
            request.Nombre,
            request.Apellido,
            request.Usuario,
            request.Contrasena,
            rolExistente.Id.ToString(),
            request.Estado ? 1 : 0);

        if (usuarioResult.IsFailure)
        {
            Result<UsuarioResponse> errorResult = Result.Failure<UsuarioResponse>(usuarioResult.Error);
            return errorResult.ToHttpResult();
        }

        var usuario = usuarioResult.Value;


        var idUsuario = await usuarioRepository.AgregarAsync(usuario, cancellationToken);

   
        var response = new UsuarioResponse(
            idUsuario,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            rolExistente.Nombre,
            usuario.Estado);

        Result<UsuarioResponse> successResult = Result.Success(response);
        return successResult.ToHttpResult();
    }
}