using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.ProductosDanados;

public sealed class EliminarProductoDanadoUseCase
{
    private readonly IProductoDanadoRepository _danados;
    private readonly IInventarioRepository _inventarios;

    public EliminarProductoDanadoUseCase(
        IProductoDanadoRepository danados,
        IInventarioRepository inventarios)
    {
        _danados = danados;
        _inventarios = inventarios;
    }

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var entidad = await _danados.ObtenerPorIdAsync(id, cancellationToken);
        if (entidad is null)
        {
            return Result.Failure(ProductoDanadoErrores.NoEncontrado(id));
        }

        var inventario = await _inventarios.ObtenerPorProductoIdAsync(entidad.ProductoId, cancellationToken);
        if (inventario is not null)
        {
            var inc = inventario.Aumentar(entidad.Cantidad.Value);
            if (inc.IsSuccess)
            {
                await _inventarios.ActualizarAsync(inventario, cancellationToken);
            }
        }

        await _danados.EliminarAsync(id, cancellationToken);
        return Result.Success();
    }
}