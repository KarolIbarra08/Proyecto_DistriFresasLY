using DistriFresasLY.Api.Contracts.Recibos;
using DistriFresasLY.Api.Endpoints.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

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
                $"No se encontro la venta #{request.VentaId} para generar el recibo."))
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

  
        var nuevoId = ReciboDataStore.RecibosDb.Count != 0 
            ? ReciboDataStore.RecibosDb.Max(r => r.Id) + 1 
            : 1;

        var consecutivo = ReciboDataStore.RecibosDb.Count != 0 
            ? ReciboDataStore.RecibosDb.Max(r => r.NumeroRecibo) + 1 
            : 1001;


        var nuevoRecibo = new ReciboModel(
            Id: nuevoId,
            FechaRecibo: DateTime.Now,
            NumeroRecibo: consecutivo,
            Total: venta.Total,
            VentaId: venta.Id
        );

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