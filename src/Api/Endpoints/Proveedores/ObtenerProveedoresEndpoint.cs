using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public class ObtenerProveedoresEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/proveedores", Manejador)
           .WithName("ObtenerProveedores")
           .WithTags("Proveedores")
           .WithSummary("Obtiene la lista de proveedores registrados");
    }

    private static async Task<IResult> Manejador(
        IProveedorRepository proveedorRepository,
        CancellationToken cancellationToken)
    {
        var proveedores = await proveedorRepository.ObtenerTodosAsync(cancellationToken);

        var listaResponse = proveedores
            .Select(proveedor => new ProveedorResponse(
                Id: proveedor.Id,
                Nombre: proveedor.Nombre,
                Apellido: proveedor.Apellido,
                CedulaNit: proveedor.Documento.Value,
                Telefono: proveedor.Telefono.Valor,
                Direccion: proveedor.Direccion,
                NombreEmpresa: proveedor.NombreEmpresa))
            .ToList();

        var result = Result.Success(listaResponse);
        return result.ToHttpResult();
    }
}