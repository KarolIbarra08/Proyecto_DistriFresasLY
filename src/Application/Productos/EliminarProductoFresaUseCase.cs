using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Application.Productos;

public sealed class EliminarProductoFresaUseCase
{
    private readonly IProductoRepository _productos;

    public EliminarProductoFresaUseCase(IProductoRepository productos) => _productos = productos;

    public async Task<Result> EjecutarAsync(int id, CancellationToken cancellationToken = default)
    {
        var producto = await _productos.ObtenerPorIdAsync(id, cancellationToken);
        if (producto is not ProductoFresa)
        {
            return Result.Failure(ProductoErrores.FresaNoEncontrada(id));
        }

        await _productos.EliminarAsync(id, cancellationToken);
        return Result.Success();
    }
}