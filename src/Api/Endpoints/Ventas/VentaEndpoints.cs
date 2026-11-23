using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Ventas;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Ventas;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Ventas;

public class VentaEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/ventas")
                       .WithTags("Ventas");

        group.MapGet("/", ObtenerTodas)
             .WithName("ObtenerVentas")
             .WithSummary("Obtiene todas las ventas registradas");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ObtenerVentaPorId")
             .WithSummary("Obtiene una venta por su ID");

        group.MapGet("/{ventaId:int}/detalles", ObtenerDetalles)
             .WithName("ObtenerDetallesVenta")
             .WithSummary("Obtiene los detalles de productos de una venta junto con el total general");

        group.MapPost("/", Crear)
             .WithName("CrearVenta")
             .WithSummary("Registra una nueva venta y descuenta el inventario");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarVenta")
             .WithSummary("Actualiza una venta existente y ajusta el inventario");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarVenta")
             .WithSummary("Elimina una venta por su ID y devuelve el inventario");
    }

    private static async Task<IResult> ObtenerTodas(
        ListarVentasUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<List<VentaResponse>>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.Select(v => v.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerVentaUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<VentaResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> ObtenerDetalles(
        int ventaId,
        ObtenerDetallesVentaUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(ventaId, cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<List<DetalleVentaResponse>>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.Select(d => d.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> Crear(
        CrearVentaRequest request,
        RegistrarVentaUseCase casoDeUso,
        CancellationToken cancellationToken,
        [FromHeader(Name = "X-Es-Admin")] bool esAdministrador = false)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(esAdministrador), cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<VentaResponse>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/ventas/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarVentaRequest request,
        ActualizarVentaUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<VentaResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarVentaUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure)
        {
            return resultado.ToHttpResult();
        }

        return Result.Success(new { mensaje = $"La venta con ID {id} se eliminó correctamente." }).ToHttpResult();
    }
}
