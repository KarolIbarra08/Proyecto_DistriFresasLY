using DistriFresasLY.Api.Contracts.Proveedores;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Proveedores;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public class ProveedorEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/proveedores")
                       .WithTags("Proveedores");

        group.MapGet("/", ObtenerTodos)
             .WithName("ObtenerProveedores")
             .WithSummary("Obtiene la lista de proveedores registrados");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ObtenerProveedorPorId")
             .WithSummary("Obtiene los datos de un proveedor existente por su ID");

        group.MapPost("/", Crear)
             .WithName("CrearProveedor")
             .WithSummary("Registra un nuevo proveedor");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarProveedor")
             .WithSummary("Actualiza los datos de un proveedor existente");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarProveedor")
             .WithSummary("Elimina un proveedor por su ID");
    }

    private static async Task<IResult> ObtenerTodos(
        ObtenerProveedoresUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<List<ProveedorResponse>>(resultado.Error).ToHttpResult();
        }

        var listaResponse = resultado.Value.Select(p => p.ToResponse()).ToList();
        return Result.Success(listaResponse).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerProveedorPorIdUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<ProveedorResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Crear(
        CrearProveedorRequest request,
        CrearProveedorUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<ProveedorResponse>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.ToResponse();
        return Result.Success(response).ToHttpCreatedAtResult($"/api/proveedores/{response.Id}");
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarProveedorRequest request,
        ActualizarProveedorUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<ProveedorResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarProveedorUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<string>(resultado.Error).ToHttpResult();
        }

        return Result.Success($"Proveedor con Id {id} eliminado correctamente.").ToHttpResult();
    }
}