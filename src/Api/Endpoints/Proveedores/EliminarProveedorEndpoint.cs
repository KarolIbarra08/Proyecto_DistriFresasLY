using DistriFresasLY.Api.Extensions;
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

    private static IResult Manejador(int id)
    {
        var proveedor = ProveedorDataStore.ProveedoresDb.FirstOrDefault(proveedor => proveedor.Id == id);
        if (proveedor is null)
        {
            Result<bool> errorResult = Error.NotFound(
                "Proveedor.NotFound",
                $"No se encontro un proveedor con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        ProveedorDataStore.ProveedoresDb.Remove(proveedor);

        Result<bool> successResult = Result.Success(true);
        return successResult.ToHttpResult();
    }
}