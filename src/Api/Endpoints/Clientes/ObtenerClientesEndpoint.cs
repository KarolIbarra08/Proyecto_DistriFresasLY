using DistriFresasLY.Api.Contracts.Clientes;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public class ObtenerClientesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clientes", Manejador)
           .WithName("ObtenerClientes")
           .WithTags("Clientes")
           .WithSummary("Obtiene la lista de clientes registrados");
    }

    private static IResult Manejador()
    {
        var clientes = ClienteDataStore.ClientesDb
            .Select(cliente => new ClienteResponse(
                Id: cliente.Id,
                Nombre: cliente.Nombre,
                CedulaNit: cliente.Documento.Value,
                Telefono: cliente.Telefono.Valor,
                Direccion: cliente.Direccion,
                TipoNegocio: cliente.TipoNegocio))
            .ToList();

        var result = Result.Success(clientes);
        return result.ToHttpResult();
    }
}