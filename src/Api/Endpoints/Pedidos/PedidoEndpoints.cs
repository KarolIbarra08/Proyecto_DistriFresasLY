using DistriFresasLY.Api.Contracts.Pedidos;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Pedidos;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pedidos;

public class PedidoEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/pedidos")
                       .WithTags("Pedidos");

        group.MapGet("/", ObtenerTodos)
             .WithName("ConsultarTodosPedidos")
             .WithSummary("Consulta todos los pedidos con sus detalles");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ConsultarPedido")
             .WithSummary("Consulta un pedido por su ID con sus detalles");

        group.MapPost("/", Registrar)
             .WithName("RegistrarPedido")
             .WithSummary("Registra un nuevo pedido con sus detalles");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarPedido")
             .WithSummary("Actualiza un pedido existente junto con sus detalles");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarPedido")
             .WithSummary("Elimina un pedido y todos sus detalles asociados");
    }

    private static async Task<IResult> ObtenerTodos(
        ObtenerPedidosUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<List<PedidoResponse>>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.Select(p => p.ToResponse()).ToList();
        return Result.Success(response).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerPedidoPorIdUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<PedidoResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Registrar(
        CrearPedidoRequest request,
        RegistrarPedidoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<PedidoResponse>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/pedidos/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarPedidoRequest request,
        ActualizarPedidoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<PedidoResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarPedidoUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        return resultado.ToHttpResult();
    }
}