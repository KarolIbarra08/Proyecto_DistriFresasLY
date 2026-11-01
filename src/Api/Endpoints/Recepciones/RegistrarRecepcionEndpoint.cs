using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class RegistrarRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/recepciones", Manejador)
           .WithName("RegistrarRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Registra una nueva recepción de pedido");
    }

    private static async Task<IResult> Manejador(
        [FromBody] RegistrarRecepcionRequest request,
        IRecepcionRepository recepcionRepository,
        CancellationToken cancellationToken)
    {
        var recepcionResult = Recepcion.Create(
            id: 0,
            fechaRecepcion: request.FechaRecepcion,
            pedidoId: request.PedidoId);

        if (recepcionResult.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(recepcionResult.Error).ToHttpResult();
        }

        var recepcion = recepcionResult.Value;
        var nuevoId = await recepcionRepository.AgregarAsync(recepcion, cancellationToken);

        var creada = await recepcionRepository.ObtenerPorIdAsync(nuevoId, cancellationToken);
        if (creada is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.Failure("Recepcion.ErrorCreacion", "No se pudo recuperar la recepción creada."))
                .ToHttpResult();
        }

        var response = new RecepcionResponse(
            creada.Id,
            creada.PedidoId,
            creada.FechaRecepcion,
            creada.ValorPago,
            creada.ProcesadaEnInventario,
            creada.CalcularTotal());

        return Result.Success(response).ToHttpCreatedAtResult($"/api/recepciones/{creada.Id}");
    }
}
