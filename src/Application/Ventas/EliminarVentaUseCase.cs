using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Ventas;

public sealed class EliminarVentaUseCase
{
    private readonly IVentaRepository _ventas;
    private readonly IInventarioRepository _inventarios;

    public EliminarVentaUseCase(
        IVentaRepository ventas,
        IInventarioRepository inventarios)
    {
        _ventas = ventas;
        _inventarios = inventarios;
    }

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var venta = await _ventas.ObtenerPorIdAsync(id, cancellationToken);
        if (venta is null)
        {
            return Result.Failure(VentaErrores.NoEncontrada(id));
        }

        if (await _ventas.TienePagosAsociadosAsync(id, cancellationToken))
        {
            return Result.Failure(VentaErrores.PagosAsociados(id));
        }

        if (await _ventas.TieneReciboAsociadoAsync(id, cancellationToken))
        {
            return Result.Failure(VentaErrores.ReciboAsociado(id));
        }

        await _ventas.EliminarAsync(id, cancellationToken);

        var cantidades = venta.Detalles
            .GroupBy(d => d.ProductoId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad.Value));

        foreach (var producto in cantidades)
        {
            var inventario = await _inventarios.ObtenerPorProductoIdAsync(producto.Key, cancellationToken);
            if (inventario is null) continue;

            var aumentarResult = inventario.Aumentar(producto.Value);
            if (aumentarResult.IsFailure) continue;

            await _inventarios.ActualizarAsync(inventario, cancellationToken);
        }

        return Result.Success();
    }
}
