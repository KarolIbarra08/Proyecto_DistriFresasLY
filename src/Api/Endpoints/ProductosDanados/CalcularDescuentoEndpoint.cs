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
           .WithSummary("Calcula el descuento del producto dañado buscando su recepción correspondiente");
    }

    private static async Task<IResult> Manejador(
        int id,
        IProductoDanadoRepository productoDanadoRepository,
        IRecepcionRepository recepcionRepository,
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


        var recepciones = await recepcionRepository.ObtenerTodosAsync(cancellationToken);
        
        var recepcionConProducto = recepciones
            .FirstOrDefault(r => r.Detalles.Any(d => d.IdProducto == entidad.ProductoId));

        if (recepcionConProducto is null)
        {
            return Result.Failure<object>(
                Error.NotFound(
                    "Recepcion.NotFound",
                    $"No se encontró ninguna recepción asociada al producto ID {entidad.ProductoId}."))
                .ToHttpResult();
        }

        var detalleProducto = recepcionConProducto.Detalles.First(d => d.IdProducto == entidad.ProductoId);


        decimal precioUnitarioDecimal = (decimal)detalleProducto.PrecioUnitario;


        var calculoResult = entidad.CalcularDescuento(precioUnitarioDecimal);
        if (calculoResult.IsFailure)
        {
            return Result.Failure<object>(calculoResult.Error).ToHttpResult();
        }


        await productoDanadoRepository.ActualizarAsync(entidad, cancellationToken);

        var response = new
        {
            entidad.Id,
            entidad.ProductoId,
            RecepcionIdAsociada = recepcionConProducto.Id,
            entidad.TipoProducto,
            Cantidad = (int)entidad.Cantidad,
            PrecioUnitario = detalleProducto.PrecioUnitario,
            entidad.FechaProductoDanado,
            entidad.Motivo,
            Descuento = entidad.Descuento is not null ? new DescuentoResponse(
                entidad.Descuento.Valor,
                entidad.Descuento.Fecha,
                entidad.Descuento.Motivo) : null
        };

        return Result.Success(response).ToHttpResult();
    }
}