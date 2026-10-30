using DistriFresasLY.Api.Contracts.Descuentos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.ProductosDanados;

public class CalcularDescuentoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/productos/danados/{id:int}/calcular-descuento", Manejador)
           .WithName("CalcularDescuento")
           .WithTags("Productos Dañados")
           .WithSummary("Calcula el descuento del producto dañado");
    }

    private static async Task<IResult> Manejador(
        int id,
        IProductoDanadoRepository productoDanadoRepository,
        CancellationToken cancellationToken)
    {
        var entidad = await productoDanadoRepository.ObtenerPorIdAsync(id, cancellationToken);

        if (entidad is null)
        {
            return Result.Failure<object>(
                Error.NotFound(
                    "ProductoDanado.NotFound",
                    $"No se encontró el producto dañado con ID: {id}"))
                .ToHttpResult();
        }

        var calculoResult = entidad.CalcularDescuento();

        if (calculoResult.IsFailure)
        {
            return Result.Failure<object>(calculoResult.Error).ToHttpResult();
        }


        await productoDanadoRepository.ActualizarAsync(entidad, cancellationToken);

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

        return Result.Success(response).ToHttpResult();
    }
}