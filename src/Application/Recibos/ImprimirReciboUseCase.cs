using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Recibos;

public sealed class ImprimirReciboUseCase
{
    private readonly IReciboRepository _recibos;

    public ImprimirReciboUseCase(IReciboRepository recibos) => _recibos = recibos;

    public async Task<Result<string>> EjecutarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var recibo = await _recibos.ObtenerPorIdAsync(id, cancellationToken);
        if (recibo is null)
        {
            return Result.Failure<string>(ReciboErrores.NoEncontrado(id));
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

        return Result.Success(reciboTexto);
    }
}
