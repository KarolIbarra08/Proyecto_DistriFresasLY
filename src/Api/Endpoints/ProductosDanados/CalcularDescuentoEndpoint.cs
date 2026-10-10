
using DistriFresasLY.Api.Contracts.Descuentos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.ProductosDanados;

namespace DistriFresasLY.Api.Endpoints.ProductosDanados;

public class CalcularDescuentoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/productos/danados/{id:int}/calcular-descuento",
                Manejador)
            .WithName("CalcularDescuento")
            .WithTags("Productos Dañados")
            .WithSummary("Calcula el descuento del producto dañado");
    }

    private static IResult Manejador(int id)
    {
       
        var dtoExistente = ProductoDanadoDataStore
            .ProductosDanadosDb
            .FirstOrDefault(p => p.Id == id);

        if (dtoExistente is null)
        {
            return Result.Failure<object>(
                Error.NotFound(
                    "ProductoDanado.NotFound",
                    $"No se encontró el producto dañado con ID: {id}"))
                .ToHttpResult();
        }

   
        var entidadResult = ProductoDanado.Create(
            dtoExistente.ProductoId,
            dtoExistente.TipoProducto,
            dtoExistente.Cantidad,
            dtoExistente.Identificacion,
            dtoExistente.Valor,
            dtoExistente.FechaProductoDanado,
            dtoExistente.Motivo,
            dtoExistente.Id);

        if (entidadResult.IsFailure)
        {
            return Result.Failure<object>(
                entidadResult.Error)
                .ToHttpResult();
        }

        var entidad = entidadResult.Value;

    
        var calculoResult = entidad.CalcularDescuento();

        if (calculoResult.IsFailure)
        {
            return Result.Failure<object>(
                calculoResult.Error)
                .ToHttpResult();
        }


        var response = new
        {
            entidad.Id,
            entidad.ProductoId,
            entidad.TipoProducto,
            entidad.Cantidad,
            entidad.Identificacion,
            entidad.Valor,
            entidad.FechaProductoDanado,
            entidad.Motivo,
            Descuento = new DescuentoResponse(
                entidad.Descuento!.Valor,
                entidad.Descuento.Fecha,
                entidad.Descuento.Motivo)
        };

        // 5. Devolver el resultado
        return Result.Success(response).ToHttpResult();
    }
}

