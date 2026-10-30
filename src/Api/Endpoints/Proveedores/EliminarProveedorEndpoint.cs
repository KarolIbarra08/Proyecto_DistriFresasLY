using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public class EliminarProveedorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/proveedores/{id:int}", Manejador)
           .WithName("EliminarProveedor")
           .WithTags("Proveedores")
           .WithSummary("Elimina un proveedor por su ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IProveedorRepository proveedorRepository,
        CancellationToken cancellationToken)
    {
        var proveedor = await proveedorRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null)
        {
            Result<string> errorResult = Error.NotFound(
                "Proveedor.NotFound",
                $"No se encontró un proveedor con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        await proveedorRepository.EliminarAsync(id, cancellationToken);

        Result<string> successResult = Result.Success($"Proveedor con Id {id} eliminado correctamente.");
        return successResult.ToHttpResult();
    }
}