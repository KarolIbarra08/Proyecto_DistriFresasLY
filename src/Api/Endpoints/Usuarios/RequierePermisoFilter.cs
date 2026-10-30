using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class RequierePermisoFilter : IEndpointFilter
{
    private readonly string _permisoRequerido;

    public RequierePermisoFilter(string permisoRequerido)
    {
        _permisoRequerido = permisoRequerido;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
      
        var rolRepository = context.HttpContext.RequestServices.GetRequiredService<IRolRepository>();

      
        if (context.HttpContext.Items.TryGetValue("RolId", out var idObj) && idObj is int rolId)
        {
            var rol = await rolRepository.ObtenerPorIdAsync(rolId, context.HttpContext.RequestAborted);
            if (rol is null || !rol.TieneAcceso(_permisoRequerido))
            {
                return Results.Problem(
                    detail: $"Acceso denegado. Se requiere el permiso '{_permisoRequerido}'.",
                    statusCode: StatusCodes.Status403Forbidden);
            }
        }

        return await next(context);
    }
}