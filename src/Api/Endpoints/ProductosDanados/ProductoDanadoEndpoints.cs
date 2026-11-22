using DistriFresasLY.Api.Contracts.Descuentos;
using DistriFresasLY.Api.Contracts.ProductosDanados;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.ProductosDanados;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.ProductosDanados;

public class ProductoDanadoEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/productos/danados")
                       .WithTags("Productos Dañados");

        group.MapGet("/", ObtenerTodos)
             .WithName("ConsultarProductosDanados")
             .WithSummary("Consulta la lista de productos dañados");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ConsultarProductoDanadoPorId")
             .WithSummary("Consulta el detalle de un producto dañado por ID");

        group.MapPost("/", Registrar)
             .WithName("RegistrarProductoDanado")
             .WithSummary("Registra un producto dañado en la base de datos y descuenta del inventario");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarProductoDanado")
             .WithSummary("Actualiza un registro de producto dañado y ajusta el inventario");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarProductoDanado")
             .WithSummary("Elimina un producto dañado por su ID y restituye el inventario");

        group.MapPost("/{id:int}/calcular-descuento", CalcularDescuento)
             .WithName("CalcularDescuento")
             .WithSummary("Calcula el descuento del producto dañado vinculándolo a una recepción específica");
    }

    private static async Task<IResult> ObtenerTodos(
        ObtenerProductosDanadosUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure) return Result.Failure<List<ProductoDanadoResponse>>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.Select(d => d.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerProductoDanadoPorIdUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoDanadoResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Registrar(
        RegistrarProductoDanadoRequest request,
        RegistrarProductoDanadoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoDanadoResponse>(resultado.Error).ToHttpResult();

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/productos/danados/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarProductoDanadoRequest request,
        ActualizarProductoDanadoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoDanadoResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarProductoDanadoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return resultado.ToHttpResult();

        return Result.Success(new { mensaje = $"El producto dañado con ID {id} se eliminó correctamente." }).ToHttpResult();
    }

    private static async Task<IResult> CalcularDescuento(
        int id,
        int? recepcionId,
        CalcularDescuentoProductoDanadoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, recepcionId, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<object>(resultado.Error).ToHttpResult();

        var d = resultado.Value;
        var response = new
        {
            d.Id,
            d.ProductoId,
            d.RecepcionIdAsociada,
            d.TipoProducto,
            d.Cantidad,
            d.PrecioUnitario,
            d.FechaProductoDanado,
            d.Motivo,
            Descuento = d.Descuento is not null
                ? new DescuentoResponse(d.Descuento.Valor, d.Descuento.Fecha, d.Descuento.Motivo)
                : null
        };

        return Result.Success<object>(response).ToHttpResult();
    }
}