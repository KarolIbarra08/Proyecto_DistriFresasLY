using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Endpoints.Pedidos;
using DistriFresasLY.Api.Extensions;
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
           .WithSummary("Registra una nueva recepción");
    }

    private static IResult Manejador(RegistrarRecepcionRequest request)
    {
        if (request.PedidoId.HasValue)
        {
            var pedido = PedidoDataStore.PedidosDb.FirstOrDefault(p => p.Id == request.PedidoId.Value);
            if (pedido is null)
            {
                return Result.Failure<RecepcionResponse>(
                    Error.NotFound("Recepcion.PedidoNoEncontrado", $"No existe el pedido con ID {request.PedidoId.Value}.")).ToHttpResult();
            }
        }

        var nuevoId = RecepcionDataStore.RecepcionesDb.Count != 0
            ? RecepcionDataStore.RecepcionesDb.Max(r => r.Id) + 1
            : 1;

        var result = Recepcion.Create(
            nuevoId,
            request.FechaRecepcion,
            request.PedidoId);

        if (result.IsFailure)
        {
            return Result.Failure<RecepcionResponse>(result.Error).ToHttpResult();
        }

        var recepcion = result.Value;
        RecepcionDataStore.RecepcionesDb.Add(recepcion);

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.PedidoId,
            recepcion.FechaRecepcion,
            recepcion.ValorPago,
            recepcion.ProcesadaEnInventario,
            recepcion.CalcularTotal());

        return Result.Success(response)
            .ToHttpCreatedAtResult($"/api/recepciones/{recepcion.Id}");
    }
}
