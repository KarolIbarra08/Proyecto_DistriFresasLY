using DistriFresasLY.Api.Contracts.Pagos.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Pagos;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Pagos;

public class PagoProveedorEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/pagos/proveedores")
                       .WithTags("Pagos - Proveedores");

        group.MapGet("/", ConsultarTodos)
             .WithName("ConsultarTodosPagosProveedores")
             .WithSummary("Consulta todos los pagos a proveedores");

        group.MapGet("/{id:int}", ConsultarPorId)
             .WithName("ConsultarPagoProveedor")
             .WithSummary("Consulta un pago de proveedor por ID");

        group.MapPost("/", Registrar)
             .WithName("RegistrarPagoProveedor")
             .WithSummary("Registra un nuevo pago a proveedor");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarPagoProveedor")
             .WithSummary("Actualiza un pago a proveedor existente");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarPagoProveedor")
             .WithSummary("Elimina un pago de proveedor por ID");
    }

    private static async Task<IResult> ConsultarTodos(
        ListarPagosProveedoresUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<List<PagoProveedorResponse>>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.Select(p => p.ToResponse()).ToList()).ToHttpResult();
    }

    private static async Task<IResult> ConsultarPorId(
        int id,
        ObtenerPagoProveedorUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<PagoProveedorResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Registrar(
        CrearPagoProveedorRequest request,
        RegistrarPagoProveedorUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<PagoProveedorResponse>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.ToResponse();
        return Result.Success(response)
            .ToHttpCreatedAtResult($"/api/pagos/proveedores/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarPagoProveedorRequest request,
        ActualizarPagoProveedorUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);
        if (resultado.IsFailure)
        {
            return Result.Failure<PagoProveedorResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarPagoProveedorUseCase casoDeUso,
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
