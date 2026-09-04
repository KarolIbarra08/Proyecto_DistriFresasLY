using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class CrearUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/usuarios", Manejador)
           .WithName("CrearUsuario")
           .WithTags("Usuarios")
           .WithSummary("Registra un nuevo usuario en el sistema");
    }

    private static IResult Manejador(CrearUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Contrasena))
        {
            Result<UsuarioResponse> validationResult = Error.Validation(
                "Usuario.Validacion",
                "El nombre de usuario y la contraseña son obligatorios.");

            return validationResult.ToHttpResult();
        }

        if (UsuarioDataStore.UsuariosDb.Any(u => u.Usuario.Equals(request.Usuario, StringComparison.OrdinalIgnoreCase)))
        {
            Result<UsuarioResponse> conflictResult = Error.Conflict(
                "Usuario.Duplicado",
                $"El usuario '{request.Usuario}' ya se encuentra registrado.");

            return conflictResult.ToHttpResult();
        }

        var nuevoId = UsuarioDataStore.UsuariosDb.Count != 0 
            ? UsuarioDataStore.UsuariosDb.Max(u => u.Id) + 1 
            : 1;

        var nuevoUsuarioModel = new UsuarioModel(
            Id: nuevoId,
            Nombre: request.Nombre.Trim(),
            Apellido: request.Apellido.Trim(),
            Usuario: request.Usuario.Trim(),
            Contrasena: request.Contrasena,
            Rol: string.IsNullOrWhiteSpace(request.Rol) ? "Usuario" : request.Rol.Trim(),
            Estado: true
        );

        UsuarioDataStore.UsuariosDb.Add(nuevoUsuarioModel);

        var response = new UsuarioResponse(
            nuevoUsuarioModel.Id,
            nuevoUsuarioModel.Nombre,
            nuevoUsuarioModel.Apellido,
            nuevoUsuarioModel.Usuario,
            nuevoUsuarioModel.Contrasena,
            nuevoUsuarioModel.Rol,
            nuevoUsuarioModel.Estado
        );

        Result<UsuarioResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/usuarios/{nuevoUsuarioModel.Id}");
    }
}