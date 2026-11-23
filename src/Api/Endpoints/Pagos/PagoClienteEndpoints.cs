using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Pagos;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos;

public class PagoClienteEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/pagos/clientes")
                       .WithTags("Pagos - Clientes");

        group.MapGet("/", ConsultarTodos)
             .WithName("ConsultarTodosPagosClientes")
             .WithSummary("Consulta la lista de todos los pagos de clientes");

        group.MapGet("/{id:int}", ConsultarPorId)
             .WithName("ConsultarPagoCliente")
             .WithSummary("Consulta un pago de cliente por su ID");

        group.MapPost("/", Registrar)
             .WithName("RegistrarPagoCliente")
             .WithSummary("Registra un abono o pago completo asociado a una venta");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarPagoCliente")
             .WithSummary("Actualiza un pago de cliente existente");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarPagoCliente")
             .WithSummary("Elimina un pago de cliente por ID");
    }

    private static async Task<IResult> ConsultarTodos(
        ListarPagosClientesUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<List<PagoClienteResponse>>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.Select(p => p.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ConsultarPorId(
        int id,
        ObtenerPagoClienteUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<PagoClienteResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Registrar(
        CrearPagoClienteRequest request,
        RegistrarPagoClienteUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<PagoClienteResponse>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.ToResponse();
        return Result.Success(response)
            .ToHttpCreatedAtResult($"/api/pagos/clientes/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarPagoClienteRequest request,
        ActualizarPagoClienteUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<PagoClienteResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarPagoClienteUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure)
        {
            return resultado.ToHttpResult();
        }

        return Result.Success(true).ToHttpResult();
    }
}
