using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Clientes;

public sealed record ObtenerClientesQuery(
    string? TipoNegocio,
    bool? Activo);

public sealed class ObtenerClientesUseCase
{
    private readonly IClienteRepository _clientes;

    public ObtenerClientesUseCase(IClienteRepository clientes)
    {
        _clientes = clientes;
    }

    public async Task<Result<IReadOnlyList<ClienteDetalle>>> EjecutarAsync(
        ObtenerClientesQuery consulta,
        CancellationToken cancellationToken = default)
    {
        var lista = await _clientes.BuscarAsync(
            consulta.TipoNegocio,
            consulta.Activo,
            cancellationToken);

        var detalles = lista.Select(c => new ClienteDetalle(
            c.Id,
            c.Nombre,
            c.Documento.Value,
            c.Telefono.Valor,
            c.Direccion,
            c.TipoNegocio,
            c.Activo)).ToList();

        return Result.Success<IReadOnlyList<ClienteDetalle>>(detalles);
    }
}