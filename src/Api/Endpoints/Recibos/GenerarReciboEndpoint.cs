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
            Result<ReciboResponse> errorResult = Error.NotFound(
                "Recibo.VentaNotFound",
                $"No se encontro la venta #{request.VentaId} para generar el recibo.");

            return errorResult.ToHttpResult();
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

        Result<ReciboResponse> createdResult = Result.Success(response);
        return createdResult.ToHttpCreatedAtResult($"/api/recibos/{nuevoRecibo.Id}");
    }
}