using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Usuarios;

public class CerrarSesionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/usuarios/logout", Manejador)
           .WithName("CerrarSesion")
           .WithTags("Usuarios")
           .WithSummary("Cierra la sesion del usuario actual");
    }

    private static IResult Manejador()
    {
        
        Result<string> successResult = Result.Success("Sesion cerrada correctamente.");
        return successResult.ToHttpResult();
    }
}