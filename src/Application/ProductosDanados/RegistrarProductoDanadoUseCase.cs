using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.ProductosDanados;

namespace DistriFresasLY.Application.ProductosDanados;

public sealed record RegistrarProductoDanadoCommand(
    int ProductoId,
    string TipoProducto,
    int Cantidad,
    string Identificacion,
    decimal Valor,
    DateTime? FechaProductoDanado,
    string Motivo);

public sealed class RegistrarProductoDanadoUseCase
{
    private readonly IProductoRepository _productos;
    private readonly IProductoDanadoRepository _danados;
    private readonly IInventarioRepository _inventarios;

    public RegistrarProductoDanadoUseCase(
        IProductoRepository productos,
        IProductoDanadoRepository danados,
        IInventarioRepository inventarios)
    {
        _productos = productos;
        _danados = danados;
        _inventarios = inventarios;
    }

    public async Task<Result<ProductoDanadoDetalle>> EjecutarAsync(
        RegistrarProductoDanadoCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (comando.ProductoId <= 0 || comando.Cantidad <= 0)
        {
            return Result.Failure<ProductoDanadoDetalle>(ProductoDanadoErrores.DatosInvalidos);
        }

        var producto = await _productos.ObtenerPorIdAsync(comando.ProductoId, cancellationToken);
        if (producto is null)
        {
            return Result.Failure<ProductoDanadoDetalle>(ProductoDanadoErrores.ProductoNoEncontrado(comando.ProductoId));
        }

        var inventario = await _inventarios.ObtenerPorProductoIdAsync(comando.ProductoId, cancellationToken);
        if (inventario is null)
        {
            return Result.Failure<ProductoDanadoDetalle>(ProductoDanadoErrores.InventarioNoEncontrado(comando.ProductoId));
        }

        var disminuirResult = inventario.Disminuir(comando.Cantidad);
        if (disminuirResult.IsFailure)
        {
            return Result.Failure<ProductoDanadoDetalle>(disminuirResult.Error);
        }

        var fechaReal = comando.FechaProductoDanado is { Year: > 1970 }
            ? comando.FechaProductoDanado.Value
            : ColombiaTime.Ahora;

        var result = ProductoDanado.Create(
            comando.ProductoId,
            comando.TipoProducto,
            comando.Cantidad,
            comando.Identificacion,
            comando.Valor,
            fechaReal,
            comando.Motivo);

        if (result.IsFailure)
        {
            return Result.Failure<ProductoDanadoDetalle>(result.Error);
        }

        await _inventarios.ActualizarAsync(inventario, cancellationToken);

        var entidad = result.Value;
        int nuevoId = await _danados.AgregarAsync(entidad, cancellationToken);

        return Result.Success(new ProductoDanadoDetalle(
            nuevoId,
            entidad.ProductoId,
            entidad.TipoProducto,
            entidad.Cantidad,
            entidad.Identificacion,
            (double)entidad.Valor,
            entidad.FechaProductoDanado,
            entidad.Motivo));
    }
}