using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class RegistrarProductoInsumoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/productos/insumo", Manejador)
           .WithName("RegistrarProductoInsumo")
           .WithTags("Productos Insumo")
           .WithSummary("Registra un nuevo producto insumo");
    }

    private static async Task<IResult> Manejador(
        CrearProductoInsumoRequest request,
        IProductoRepository productoRepository,
        CancellationToken cancellationToken)
    {
        // 1. Crear el Value Object/Entidad con las validaciones de Dominio
        var result = ProductoInsumo.Create(
            request.Nombre,
            request.TipoInsumo,
            request.UnidadMedida,
            request.Descripcion);

        if (result.IsFailure)
        {
            return Result
                .Failure<ProductoInsumoResponse>(result.Error)
                .ToHttpResult();
        }

        var insumo = result.Value;

        // 2. Guardar permanentemente en la Base de Datos SQLite
        var idGenerado = await productoRepository.AgregarInsumoAsync(insumo, cancellationToken);

        // 3. Crear el objeto de respuesta con el ID persistido
        var response = new ProductoInsumoResponse(
            idGenerado,
            insumo.TipoProducto,
            insumo.Nombre,
            insumo.TipoInsumo,
            insumo.UnidadMedida,
            insumo.Descripcion
        );

        return Result
            .Success(response)
            .ToHttpCreatedAtResult($"/api/productos/insumo/{idGenerado}");
    }
}