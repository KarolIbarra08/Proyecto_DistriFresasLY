using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class EliminarClienteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/clientes/{id:int}", Manejador)
           .WithName("EliminarCliente")
           .WithTags("Clientes")
           .WithSummary("Elimina un cliente por ID");
    }

    private static async Task<IResult> Manejador(
        int id,
        IClienteRepository clienteRepository,
        CancellationToken cancellationToken)
    {
        // 1. Verificar si el cliente existe
        var cliente = await clienteRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return Result.Failure(
                Error.NotFound("Cliente.NotFound", $"No se encontró el cliente con ID: {id}")
            ).ToHttpResult();
        }

        // 2. Verificar si el cliente tiene pedidos vinculados
        var tienePedidos = await clienteRepository.TienePedidosAsociadosAsync(id, cancellationToken);
        if (tienePedidos)
        {
            return Result.Failure(
                Error.Conflict(
                    "Cliente.TienePedidos", 
                    $"No se puede eliminar el cliente con ID {id} porque tiene pedidos asociados.")
            ).ToHttpResult();
        }

        // 3. Proceder con la eliminación si no tiene dependencias
        await clienteRepository.EliminarAsync(id, cancellationToken);

        var respuesta = new 
        { 
            mensaje = $"El cliente con ID {id} se eliminó correctamente." 
        };

        return Result.Success(respuesta).ToHttpResult();
    }
}