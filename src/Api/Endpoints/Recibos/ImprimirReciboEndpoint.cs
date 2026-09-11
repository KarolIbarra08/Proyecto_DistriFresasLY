using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recibos;

public class ImprimirReciboEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recibos/{id:int}/imprimir", Manejador)
           .WithName("ImprimirRecibo")
           .WithTags("Recibos")
           .WithSummary("Obtiene la plantilla formateada para impresion de un recibo");
    }

    private static IResult Manejador(int id)
    {
        var recibo = ReciboDataStore.RecibosDb.FirstOrDefault(r => r.Id == id);
        if (recibo is null)
        {
            Result<string> errorResult = Error.NotFound(
                "Recibo.NotFound",
                $"No se encontro el recibo con Id: {id}");

            return errorResult.ToHttpResult();
        }

        var reciboTexto = $"""
        ========================================
                   DISTRIFRESAS LY              
               COMPROBANTE DE RECIBO            
        ========================================
        No. Recibo   : #{recibo.NumeroRecibo}
        Fecha        : {recibo.FechaRecibo:dd/MM/yyyy HH:mm}
        ID Venta     : {recibo.VentaId}
        ----------------------------------------
        TOTAL PAGADO : ${recibo.Total:N2}
        ========================================
               ¡Gracias por su compra!          
        """;

        Result<string> successResult = Result.Success(reciboTexto);
        return successResult.ToHttpResult();
    }
}