using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.DetalleRecepciones;
using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Recepciones;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class RecepcionEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/recepciones")
                       .WithTags("Recepciones");

        group.MapGet("/", Listar)
             .WithName("ListarRecepciones")
             .WithSummary("Lista todas las recepciones registradas");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ObtenerRecepcion")
             .WithSummary("Obtiene una recepción por su ID aplicando deducciones por productos dañados");

        group.MapPost("/", Registrar)
             .WithName("RegistrarRecepcion")
             .WithSummary("Registra una nueva recepción de pedido");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarRecepcion")
             .WithSummary("Actualiza los datos de una recepción existente");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarRecepcion")
             .WithSummary("Elimina una recepción por su ID");

        group.MapPost("/{id:int}/procesar", Procesar)
             .WithName("ProcesarRecepcion")
             .WithSummary("Marca una recepción como procesada e incrementa el inventario");

        group.MapGet("/{id:int}/total", CalcularTotal)
             .WithName("CalcularTotalRecepcion")
             .WithSummary("Calcula el valor total de una recepción");

        group.MapGet("/{id:int}/detalles", ObtenerDetalles)
             .WithName("ObtenerDetallesRecepcion")
             .WithSummary("Muestra los detalles de una recepción");
    }

    private static async Task<IResult> Listar(
        ListarRecepcionesUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure) return Result.Failure<List<RecepcionResponse>>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.Select(r => r.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<RecepcionResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Registrar(
        [FromBody] RegistrarRecepcionRequest request,
        RegistrarRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<RecepcionResponse>(resultado.Error).ToHttpResult();

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/recepciones/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarRecepcionRequest request,
        ActualizarRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<RecepcionResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        return resultado.ToHttpResult();
    }

    private static async Task<IResult> Procesar(
        int id,
        ProcesarRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<RecepcionResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> CalcularTotal(
        int id,
        CalcularTotalRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        return resultado.ToHttpResult();
    }

    private static async Task<IResult> ObtenerDetalles(
        int id,
        ObtenerDetallesRecepcionUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<List<DetalleRecepcionResponse>>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.Select(d => d.ToResponse()).ToList()).ToHttpResult();
    }
}