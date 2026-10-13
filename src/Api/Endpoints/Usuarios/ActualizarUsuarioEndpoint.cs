using DistriFresasLY.Api.Contracts.Usuarios;
using DistriFresasLY.Api.Endpoints.Roles; // Necesario para consultar RolDataStore
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Usuarios;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class ActualizarUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/usuarios/{id:int}", Manejador)
           .WithName("ActualizarUsuario")
           .WithTags("Usuarios")
           .WithSummary("Actualiza los datos de un usuario existente");
    }

    private static IResult Manejador(int id, ActualizarUsuarioRequest request)
    {
        // 1. Buscar si el usuario a actualizar existe
        var usuario = UsuarioDataStore.UsuariosDb.FirstOrDefault(u => u.Id == id);
        if (usuario is null)
        {
            Result<UsuarioResponse> errorResult = Error.NotFound(
                "Usuario.NotFound",
                $"No se encontró un usuario con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        // 2. Determinar valores propuesta (fallback a los actuales si vienen vacíos)
        var nuevoNombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre : usuario.Nombre;
        var nuevoApellido = !string.IsNullOrWhiteSpace(request.Apellido) ? request.Apellido : usuario.Apellido;
        var nuevoNombreUsuario = !string.IsNullOrWhiteSpace(request.Usuario) ? request.Usuario : usuario.NombreUsuario;
        var nuevaContrasena = !string.IsNullOrWhiteSpace(request.Contrasena) ? request.Contrasena : usuario.Contrasena;
        var nuevoRol = !string.IsNullOrWhiteSpace(request.Rol) ? request.Rol : usuario.Rol;

        // 3. Validación de Infraestructura: Si cambia el nombre de usuario, verificar que no esté ocupado por OTRO usuario
        if (!nuevoNombreUsuario.Equals(usuario.NombreUsuario, StringComparison.OrdinalIgnoreCase) &&
            UsuarioDataStore.UsuariosDb.Any(u => u.Id != id && u.NombreUsuario.Equals(nuevoNombreUsuario, StringComparison.OrdinalIgnoreCase)))
        {
            Result<UsuarioResponse> conflictResult = Error.Conflict(
                "Usuario.Duplicado",
                $"El nombre de usuario '{nuevoNombreUsuario}' ya está en uso por otro usuario.");

            return conflictResult.ToHttpResult();
        }

        // 4. Regla de Negocio: Verificar que el ROL asignado realmente exista en la base de datos
        bool existeRol = RolDataStore.RolesDb.Any(r => r.Nombre.Equals(nuevoRol, StringComparison.OrdinalIgnoreCase));
        if (!existeRol)
        {
            Result<UsuarioResponse> rolNotFoundResult = Error.NotFound(
                "Usuario.RolNoEncontrado",
                $"El rol '{nuevoRol}' especificado no existe en el sistema.");

            return rolNotFoundResult.ToHttpResult();
        }

        // 5. Delegar la actualización y validaciones de campo al DOMINIO
        var actualizarResult = usuario.ActualizarDatos(nuevoNombre, nuevoApellido, nuevoNombreUsuario, nuevaContrasena, nuevoRol);
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<UsuarioResponse>(actualizarResult.Error).ToHttpResult();
        }

        // 6. Retornar respuesta mapeada
        var response = new UsuarioResponse(
            usuario.Id,
            usuario.Nombre,
            usuario.Apellido,
            usuario.NombreUsuario,
            usuario.Contrasena,
            usuario.Rol,
            usuario.Estado);

        return Result.Success(response).ToHttpResult();
    }
}