using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public class ObtenerProveedorPorIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/proveedores/{id:int}", Manejador)
           .WithName("ObtenerProveedorPorId")
           .WithTags("Proveedores")
           .WithSummary("Obtiene los datos de un proveedor existente por su ID");
    }

    private static async Task<IResult> Manejador(
        int id,
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

        var response = new ProveedorResponse(
            Id: proveedor.Id,
            Nombre: proveedor.Nombre,
            Apellido: proveedor.Apellido,
            CedulaNit: proveedor.Documento.Value,
            Telefono: proveedor.Telefono.Valor,
            Direccion: proveedor.Direccion,
            NombreEmpresa: proveedor.NombreEmpresa
        );

        var result = Result.Success(response);
        return result.ToHttpResult();
    }
}