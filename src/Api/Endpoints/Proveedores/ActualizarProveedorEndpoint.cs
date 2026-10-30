using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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

    private static async Task<IResult> Manejador(
        int id,
        ActualizarProveedorRequest request,
        IProveedorRepository proveedorRepository,
        CancellationToken cancellationToken)
    {
        var proveedor = await proveedorRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null)
        {
            Result<ProveedorResponse> errorResult = Error.NotFound(
                "Proveedor.NotFound",
                $"No se encontró un proveedor con el Id: {id}");

            return errorResult.ToHttpResult();
        }

        if (!string.IsNullOrWhiteSpace(request.CedulaNit) &&
            !request.CedulaNit.Trim().Equals(proveedor.Documento.Value, StringComparison.OrdinalIgnoreCase))
        {
            var dupl = await proveedorRepository.ObtenerPorCedulaNitAsync(request.CedulaNit, cancellationToken);
            if (dupl is not null && dupl.Id != id)
            {
                return Result.Failure<ProveedorResponse>(
                    Error.Conflict("Proveedor.CedulaDuplicada", $"La Cédula/NIT '{request.CedulaNit}' ya está registrada.")
                ).ToHttpResult();
            }

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

        await proveedorRepository.ActualizarAsync(proveedor, cancellationToken);

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