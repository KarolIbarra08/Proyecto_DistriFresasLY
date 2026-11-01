using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Recibos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
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
           .WithSummary("Genera un recibo digital tomando los datos de una venta");
    }

    private static async Task<IResult> Manejador(
        [FromBody] GenerarReciboRequest request,
        IReciboRepository reciboRepository,
        IVentaRepository ventaRepository,
        CancellationToken cancellationToken)
    {
        var venta = await ventaRepository.ObtenerPorIdAsync(request.VentaId, cancellationToken);
        if (venta is null)
        {
            return Result.Failure<ReciboResponse>(Error.NotFound(
                "Recibo.VentaNotFound",
                $"No se encontró la venta #{request.VentaId} para generar el recibo."))
                .ToHttpResult();
        }

        var recibos = await reciboRepository.ObtenerTodosAsync(cancellationToken);

        var reciboExistente = recibos.FirstOrDefault(r => r.VentaId == request.VentaId);
        if (reciboExistente is not null)
        {
            return Result.Failure<ReciboResponse>(Error.Conflict(
                "Recibo.AlreadyExists",
                $"Ya existe un recibo (No. #{reciboExistente.NumeroRecibo}) generado para la venta #{request.VentaId}."))
                .ToHttpResult();
        }

        var ultimoNumero = recibos.Count > 0 ? recibos.Max(r => r.NumeroRecibo) : 0;

        var reciboResult = Recibo.Create(request.VentaId, venta.Total, ultimoNumero);
        if (reciboResult.IsFailure)
        {
            return Result.Failure<ReciboResponse>(reciboResult.Error).ToHttpResult();
        }

        var nuevoRecibo = reciboResult.Value;
        var nuevoId = await reciboRepository.AgregarAsync(nuevoRecibo, cancellationToken);

        var creado = await reciboRepository.ObtenerPorIdAsync(nuevoId, cancellationToken);
        if (creado is null)
        {
            return Result.Failure<ReciboResponse>(
                Error.Failure("Recibo.ErrorCreacion", "No se pudo recuperar el recibo creado."))
                .ToHttpResult();
        }

        var response = new ReciboResponse(
            creado.Id,
            creado.FechaRecibo,
            creado.NumeroRecibo,
            creado.Total,
            creado.VentaId);

        return Result.Success(response).ToHttpCreatedAtResult($"/api/recibos/{creado.Id}");
    }
}
