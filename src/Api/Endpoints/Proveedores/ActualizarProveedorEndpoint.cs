using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Proveedores;

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
                $"No se encontro un proveedor con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        var proveedor = ProveedorDataStore.ProveedoresDb[index];

        if (!string.IsNullOrWhiteSpace(request.CedulaNit))
        {
            var documentoResult = proveedor.ActualizarDocumento(request.CedulaNit.Trim());
            if (documentoResult.IsFailure)
            {
                return Result.Failure<ProveedorResponse>(documentoResult.Error).ToHttpResult();
            }
        }

        var nombre = !string.IsNullOrWhiteSpace(request.Nombre) ? request.Nombre.Trim() : proveedor.Nombre;
        var apellido = !string.IsNullOrWhiteSpace(request.Apellido) ? request.Apellido.Trim() : proveedor.Apellido;
        var telefono = !string.IsNullOrWhiteSpace(request.Telefono) ? request.Telefono.Trim() : proveedor.Telefono.Valor;
        var direccion = !string.IsNullOrWhiteSpace(request.Direccion) ? request.Direccion.Trim() : proveedor.Direccion;
        var nombreEmpresa = !string.IsNullOrWhiteSpace(request.NombreEmpresa) ? request.NombreEmpresa.Trim() : proveedor.NombreEmpresa;

        var actualizarResult = proveedor.ActualizarDatos(nombre, apellido, telefono, direccion, nombreEmpresa);
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<ProveedorResponse>(actualizarResult.Error).ToHttpResult();
        }

        var response = new ProveedorResponse(
            Id: proveedor.Id,
            Nombre: proveedor.Nombre,
            Apellido: proveedor.Apellido,
            CedulaNit: proveedor.Documento.Value,
            Telefono: proveedor.Telefono.Valor,
            Direccion: proveedor.Direccion,
            NombreEmpresa: proveedor.NombreEmpresa
        );

        Result<ProveedorResponse> successResult = Result.Success(response);
        return successResult.ToHttpResult();
    }
}