using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public class ActualizarProveedorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/proveedores/{id:int}", Manejador)
           .WithName("ActualizarProveedor")
           .WithTags("Proveedores")
           .WithSummary("Actualiza los datos de un proveedor existente");
    }

    private static IResult Manejador(int id, ActualizarProveedorRequest request)
    {
        var index = ProveedorDataStore.ProveedoresDb.FindIndex(proveedor => proveedor.Id == id);
        if (index == -1)
        {
            Result<ProveedorResponse> errorResult = Error.NotFound(
                "Proveedor.NotFound",
                $"No se encontró un proveedor con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var proveedorExistente = ProveedorDataStore.ProveedoresDb[index];

        
        var proveedorActualizado = proveedorExistente with
        {
            Nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : proveedorExistente.Nombre,
            Apellido = !string.IsNullOrWhiteSpace(request.Apellido) ? request.Apellido.Trim() : proveedorExistente.Apellido,
            CedulaNit = !string.IsNullOrWhiteSpace(request.CedulaNit) ? request.CedulaNit.Trim() : proveedorExistente.CedulaNit,
            Telefono = !string.IsNullOrWhiteSpace(request.Telefono) ? request.Telefono.Trim() : proveedorExistente.Telefono,
            Direccion = !string.IsNullOrWhiteSpace(request.Direccion) ? request.Direccion.Trim() : proveedorExistente.Direccion,
            NombreEmpresa = !string.IsNullOrWhiteSpace(request.NombreEmpresa) ? request.NombreEmpresa.Trim() : proveedorExistente.NombreEmpresa
        };

        ProveedorDataStore.ProveedoresDb[index] = proveedorActualizado;

        Result<ProveedorResponse> successResult = Result.Success(proveedorActualizado);
        return successResult.ToHttpResult();
    }
}