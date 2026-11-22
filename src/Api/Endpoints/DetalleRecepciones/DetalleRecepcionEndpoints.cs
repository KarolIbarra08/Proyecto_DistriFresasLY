using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.DetalleRecepciones;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.DetalleRecepciones;

public class DetalleRecepcionEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/detalles-recepcion")
                       .WithTags("DetalleRecepciones");

        group.MapGet("/", Listar)
             .WithName("ListarDetallesRecepcion")
             .WithSummary("Muestra todos los detalles de recepción");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ObtenerDetalleRecepcion")
             .WithSummary("Obtiene un detalle de recepción por ID");

        group.MapPost("/", Registrar)
             .WithName("RegistrarDetalleRecepcion")
             .WithSummary("Registra un detalle asociado a una recepción y actualiza el inventario");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarDetalleRecepcion")
             .WithSummary("Actualiza un detalle de recepción");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarDetalleRecepcion")
             .WithSummary("Elimina un detalle y actualiza el inventario");
    }

    private static async Task<IResult> Listar(
        ListarDetallesRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure) return Result.Failure<List<DetalleRecepcionResponse>>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.Select(d => d.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerDetalleRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<DetalleRecepcionResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Registrar(
        RegistrarDetalleRecepcionRequest request,
        RegistrarDetalleRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<DetalleRecepcionResponse>(resultado.Error).ToHttpResult();

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/detalles-recepcion/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarDetalleRecepcionRequest request,
        ActualizarDetalleRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<DetalleRecepcionResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarDetalleRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        return resultado.ToHttpResult();
    }
}