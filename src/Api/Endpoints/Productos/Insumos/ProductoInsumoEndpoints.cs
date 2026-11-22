using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Productos;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Insumos;

public class ProductoInsumoEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/productos/insumo")
                       .WithTags("Productos Insumo");

        group.MapGet("/", ObtenerTodos)
             .WithName("ConsultarProductosInsumo")
             .WithSummary("Consulta la lista de productos insumo");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ConsultarProductoInsumoPorId")
             .WithSummary("Consulta un producto insumo por ID");

        group.MapPost("/", Registrar)
             .WithName("RegistrarProductoInsumo")
             .WithSummary("Registra un nuevo producto insumo");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarProductoInsumo")
             .WithSummary("Actualiza los datos de un producto insumo y retorna el resultado");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarProductoInsumo")
             .WithSummary("Elimina un producto insumo por su ID");
    }

    private static async Task<IResult> ObtenerTodos(
        ObtenerProductosInsumoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure) return Result.Failure<List<ProductoInsumoResponse>>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.Select(i => i.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerProductoInsumoPorIdUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoInsumoResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Registrar(
        CrearProductoInsumoRequest request,
        RegistrarProductoInsumoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoInsumoResponse>(resultado.Error).ToHttpResult();

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/productos/insumo/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarProductoInsumoRequest request,
        ActualizarProductoInsumoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoInsumoResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarProductoInsumoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return resultado.ToHttpResult();

        return Result.Success(new { mensaje = $"El producto insumo con ID {id} se eliminó correctamente." }).ToHttpResult();
    }
}