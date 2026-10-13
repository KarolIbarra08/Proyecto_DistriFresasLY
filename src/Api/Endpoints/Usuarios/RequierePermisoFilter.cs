using DistriFresasLY.Api.Endpoints.Roles;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Filters;

public static class PermisoFilterExtensions
{
    public static RouteHandlerBuilder RequierePermiso(this RouteHandlerBuilder builder, string permiso)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
           
            var rolHeader = context.HttpContext.Request.Headers["X-Rol-Usuario"].ToString();

            if (string.IsNullOrWhiteSpace(rolHeader))
            {
                var errorUnauthorized = Error.Validation("Auth.SinRol", "No se especificó el rol en la cabecera X-Rol-Usuario.");
                return Results.Json(errorUnauthorized, statusCode: StatusCodes.Status401Unauthorized);
            }

          
            var rolObjeto = RolDataStore.RolesDb.FirstOrDefault(r => 
                r.Nombre.Equals(rolHeader, StringComparison.OrdinalIgnoreCase));

            
            if (rolObjeto is null || !rolObjeto.TieneAcceso(permiso))
            {
                var errorForbidden = Error.Validation("Auth.AccesoDenegado", $"No tienes permiso para ejecutar esta operación. Requiere: '{permiso}'");
                return Results.Json(errorForbidden, statusCode: StatusCodes.Status403Forbidden);
            }

            return await next(context);
        });
    }
}