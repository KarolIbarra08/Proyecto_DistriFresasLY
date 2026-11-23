using DistriFresasLY.Api.Contracts.Recibos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Recibos;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recibos;

public class ReciboEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/recibos")
                       .WithTags("Recibos");

        group.MapGet("/", ObtenerTodos)
             .WithName("ObtenerRecibos")
             .WithSummary("Obtiene la lista completa de recibos generados");

        group.MapPost("/generar", Generar)
             .WithName("GenerarRecibo")
             .WithSummary("Genera un recibo digital tomando los datos de una venta");

        group.MapGet("/{id:int}/imprimir", Imprimir)
             .WithName("ImprimirRecibo")
             .WithSummary("Obtiene la plantilla formateada para impresion de un recibo");
    }

    private static async Task<IResult> ObtenerTodos(
        ListarRecibosUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<List<ReciboResponse>>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.Select(r => r.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> Generar(
        GenerarReciboRequest request,
        GenerarReciboUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<ReciboResponse>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/recibos/{response.Id}");
    }

    private static async Task<IResult> Imprimir(
        int id,
        ImprimirReciboUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<string>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value).ToHttpResult();
    }
}
