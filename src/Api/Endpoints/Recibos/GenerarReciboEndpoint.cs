using DistriFresasLY.Api.Contracts.Recibos;
using DistriFresasLY.Api.Endpoints.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recibos;

namespace DistriFresasLY.Api.Endpoints.Recibos;

public class GenerarReciboEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/recibos/generar", Manejador)
           .WithName("GenerarRecibo")
           .WithTags("Recibos")
           .WithSummary("Genera un recibo digital tomando los datos directos de una venta");
    }

    private static IResult Manejador(GenerarReciboRequest request)
    {
        var venta = VentaDataStore.VentasDb.FirstOrDefault(v => v.Id == request.VentaId);
        if (venta is null)
        {
            return Result.Failure<ReciboResponse>(Error.NotFound(
                "Recibo.VentaNotFound",
                $"No se encontró la venta #{request.VentaId} para generar el recibo."))
                .ToHttpResult();
        }

        var reciboExistente = ReciboDataStore.RecibosDb.FirstOrDefault(r => r.VentaId == request.VentaId);
        if (reciboExistente is not null)
        {
            return Result.Failure<ReciboResponse>(Error.Conflict(
                "Recibo.AlreadyExists",
                $"Ya existe un recibo (No. #{reciboExistente.NumeroRecibo}) generado para la venta #{request.VentaId}."))
                .ToHttpResult();
        }

        var ultimoNumero = ReciboDataStore.RecibosDb.Count != 0 
            ? ReciboDataStore.RecibosDb.Max(r => r.NumeroRecibo) 
            : 0;

        var nuevoId = ReciboDataStore.RecibosDb.Count != 0 
            ? ReciboDataStore.RecibosDb.Max(r => r.Id) + 1 
            : 1;

        // Creación encapsulada desde el Dominio
        var reciboResult = Recibo.Create(venta.Id, venta.Total, ultimoNumero, nuevoId);
        if (reciboResult.IsFailure)
        {
            return Result.Failure<ReciboResponse>(reciboResult.Error).ToHttpResult();
        }

        var nuevoRecibo = reciboResult.Value;
        ReciboDataStore.RecibosDb.Add(nuevoRecibo);

        var response = new ReciboResponse(
            nuevoRecibo.Id,
            nuevoRecibo.FechaRecibo,
            nuevoRecibo.NumeroRecibo,
            nuevoRecibo.Total,
            nuevoRecibo.VentaId
        );

        return Result.Success(response).ToHttpCreatedAtResult($"/api/recibos/{nuevoRecibo.Id}");
    }
}