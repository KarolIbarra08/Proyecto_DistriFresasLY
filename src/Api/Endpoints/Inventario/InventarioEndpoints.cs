using Microsoft.AspNetCore.Mvc;
using DistriFresasLY.Api.Contracts.Inventario;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Inventarios;
using DistriFresasLY.Domain.Common;
namespace DistriFresasLY.Api.Endpoints.Inventario;

public class InventarioEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/inventario")
                       .WithTags("Inventario");

        group.MapGet("/", ListarTodo)
             .WithName("ListarInventario")
             .WithSummary("Consulta el stock disponible de todos los productos");

        group.MapGet("/{productoId:int}", ConsultarPorProducto)
             .WithName("ConsultarStock")
             .WithSummary("Consulta el stock disponible de un producto");

        group.MapPost("/ajustar", Ajustar)
             .WithName("AjustarStock")
             .WithSummary("Ajusta directamente el stock de un producto en el inventario");

        group.MapPost("/aumentar", Aumentar)
             .WithName("AumentarStock")
             .WithSummary("Registra un incremento en el stock del inventario");

        group.MapPost("/disminuir", Disminuir)
             .WithName("DisminuirStock")
             .WithSummary("Registra una salida de stock del inventario");
    }

    private static async Task<IResult> ListarTodo(
        ConsultarTodoInventarioUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure) return Result.Failure<List<InventarioResponse>>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.Select(i => i.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ConsultarPorProducto(
        int productoId,
        ConsultarStockUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(productoId, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<InventarioResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Ajustar(
        [FromBody] ActualizarStockRequest request,
        AjustarStockUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<InventarioResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Aumentar(
        [FromBody] ActualizarStockRequest request,
        AumentarStockUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<InventarioResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Disminuir(
        [FromBody] ActualizarStockRequest request,
        DisminuirStockUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<InventarioResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }
}