using DistriFresasLY.Api.Contracts.Productos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Productos;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Productos.Fresas;

public class ProductoFresaEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/productos/fresa")
                       .WithTags("Productos Fresa");

        group.MapGet("/", ObtenerTodas)
             .WithName("ConsultarTodosLosProductosFresas")
             .WithSummary("Consulta todos los productos fresa");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ConsultarProductoFresaPorId")
             .WithSummary("Consulta un producto fresa por ID");

        group.MapPost("/", Registrar)
             .WithName("RegistrarProductoFresa")
             .WithSummary("Registra un nuevo producto fresa");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarProductoFresa")
             .WithSummary("Actualiza un producto fresa existente");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarProductoFresa")
             .WithSummary("Elimina un producto fresa por ID");
    }

    private static async Task<IResult> ObtenerTodas(
        ObtenerProductosFresaUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure) return Result.Failure<List<ProductoFresaResponse>>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.Select(f => f.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerProductoFresaPorIdUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoFresaResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Registrar(
        CrearProductoFresaRequest request,
        RegistrarProductoFresaUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoFresaResponse>(resultado.Error).ToHttpResult();

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/productos/fresa/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarProductoFresaRequest request,
        ActualizarProductoFresaUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);
        if (resultado.IsFailure) return Result.Failure<ProductoFresaResponse>(resultado.Error).ToHttpResult();

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarProductoFresaUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure) return resultado.ToHttpResult();

        return Result.Success(new { mensaje = $"El producto fresa con ID {id} se eliminó correctamente." }).ToHttpResult();
    }
}