using DistriFresasLY.Api.Contracts.Descuentos;
using DistriFresasLY.Api.Contracts.ProductosDanados;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.ProductosDanados;

namespace DistriFresasLY.Api.Endpoints.ProductosDanados;

public class CalcularDescuentoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/productos/danados/{id:int}/calcular-descuento", Manejador)
           .WithName("CalcularDescuento")
           .WithTags("Productos Dañados")
           .WithSummary("Calcula el descuento del producto dañado invocando las reglas de dominio");
    }

    private static IResult Manejador(int id)
    {
        var index = ProductoDanadoDataStore.ProductosDanadosDb.FindIndex(p => p.Id == id);
        if (index == -1)
        {
            return Result.Failure<ProductoDanadoResponse>(Error.NotFound(
                "ProductoDanado.NotFound", $"No se encontro el producto dañado con ID: {id}"))
                .ToHttpResult();
        }

        var dtoExistente = ProductoDanadoDataStore.ProductosDanadosDb[index];

        var entidadResult = ProductoDanado.Create(
            dtoExistente.ProductoId,
            dtoExistente.TipoProducto,
            dtoExistente.Cantidad,
            dtoExistente.Identificacion,
            dtoExistente.Valor,
            dtoExistente.FechaProductoDanado,
            dtoExistente.Motivo,
            dtoExistente.Id
        );

        if (entidadResult.IsFailure)
            return Result.Failure<ProductoDanadoResponse>(entidadResult.Error).ToHttpResult();

        var entidad = entidadResult.Value;


        var calculoResult = entidad.CalcularDescuento();
        if (calculoResult.IsFailure)
            return Result.Failure<ProductoDanadoResponse>(calculoResult.Error).ToHttpResult();

        var response = new ProductoDanadoResponse(
            entidad.Id,
            entidad.ProductoId,
            entidad.TipoProducto,
            entidad.Cantidad,
            entidad.Identificacion,
            entidad.Valor,
            entidad.FechaProductoDanado,
            entidad.Motivo,
            new DescuentoResponse(entidad.Descuento!.Valor, entidad.Descuento.Fecha, entidad.Descuento.Motivo)
        );

        ProductoDanadoDataStore.ProductosDanadosDb[index] = response;

        return Result.Success(response).ToHttpResult();
    }
}