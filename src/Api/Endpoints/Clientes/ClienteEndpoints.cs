using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Clientes;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class ClienteEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/clientes")
                       .WithTags("Clientes");

        group.MapGet("/", ObtenerTodos)
             .WithName("ObtenerClientes")
             .WithSummary("Obtiene la lista de clientes registrados. Filtros opcionales: tipoNegocio y activo");

        group.MapGet("/{id:int}", ObtenerPorId)
             .WithName("ObtenerClientePorId")
             .WithSummary("Obtiene el detalle de un cliente por su ID");

        group.MapPost("/", Crear)
             .WithName("CrearCliente")
             .WithSummary("Crea un nuevo cliente validando unicidad de Cédula/NIT");

        group.MapPut("/{id:int}", Actualizar)
             .WithName("ActualizarCliente")
             .WithSummary("Actualiza la información de un cliente existente");

        group.MapDelete("/{id:int}", Eliminar)
             .WithName("EliminarCliente")
             .WithSummary("Elimina un cliente si no tiene pedidos asociados");
    }

    private static async Task<IResult> ObtenerTodos(
        string? tipoNegocio,
        bool? activo,
        ObtenerClientesUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(
            new ObtenerClientesQuery(tipoNegocio, activo),
            cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<ClienteResponse[]>(resultado.Error).ToHttpResult();
        }

        var response = resultado.Value.Select(c => c.ToResponse()).ToArray();
        return Result.Success(response).ToHttpResult();
    }

    private static async Task<IResult> ObtenerPorId(
        int id,
        ObtenerClientePorIdUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<ClienteResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Crear(
        CrearClienteRequest request,
        CrearClienteUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<ClienteResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Actualizar(
        int id,
        ActualizarClienteRequest request,
        ActualizarClienteUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(request.ToCommand(id), cancellationToken);

        if (resultado.IsFailure)
        {
            return Result.Failure<ClienteResponse>(resultado.Error).ToHttpResult();
        }

        return Result.Success(resultado.Value.ToResponse()).ToHttpResult();
    }

    private static async Task<IResult> Eliminar(
        int id,
        EliminarClienteUseCase casoDeUso,
        CancellationToken cancellationToken)
    {
        var resultado = await casoDeUso.EjecutarAsync(id, cancellationToken);
        return resultado.ToHttpResult();
    }
}