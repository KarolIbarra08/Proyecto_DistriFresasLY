using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Ventas;

namespace DistriFresasLY.Application.Ventas;

public sealed record CrearDetalleVentaCommand(
    int ProductoId,
    int Cantidad,
    double PrecioUnitario);

public sealed record RegistrarVentaCommand(
    int? ClienteId,
    IReadOnlyList<CrearDetalleVentaCommand>? Detalles,
    bool EsAdministrador);

public sealed class RegistrarVentaUseCase
{
    private readonly IVentaRepository _ventas;
    private readonly IClienteRepository _clientes;
    private readonly IInventarioRepository _inventarios;

    public RegistrarVentaUseCase(
        IVentaRepository ventas,
        IClienteRepository clientes,
        IInventarioRepository inventarios)
    {
        _ventas = ventas;
        _clientes = clientes;
        _inventarios = inventarios;
    }

    public async Task<Result<VentaDetalleDto>> EjecutarAsync(
        RegistrarVentaCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (comando.ClienteId is null || comando.ClienteId.Value <= 0)
        {
            return Result.Failure<VentaDetalleDto>(VentaErrores.ClienteRequerido);
        }

        var cliente = await _clientes.ObtenerPorIdAsync(comando.ClienteId.Value, cancellationToken);
        if (cliente is null)
        {
            return Result.Failure<VentaDetalleDto>(
                VentaErrores.ClienteNoEncontrado(comando.ClienteId.Value));
        }

        if (comando.Detalles is null || comando.Detalles.Count == 0)
        {
            return Result.Failure<VentaDetalleDto>(VentaErrores.SinDetalles);
        }

        var detalles = new List<DetalleVenta>();
        var stockPorProducto = new Dictionary<int, int>();

        foreach (var detReq in comando.Detalles)
        {
            var inventario = await _inventarios.ObtenerPorProductoIdAsync(detReq.ProductoId, cancellationToken);
            if (inventario is null)
            {
                return Result.Failure<VentaDetalleDto>(
                    VentaErrores.InventarioNoEncontrado(detReq.ProductoId));
            }

            stockPorProducto[detReq.ProductoId] = inventario.CantidadDisponible;

            var detalleResult = DetalleVenta.Create(
                productoId: detReq.ProductoId,
                cantidad: detReq.Cantidad,
                precioUnitario: detReq.PrecioUnitario);

            if (detalleResult.IsFailure)
            {
                return Result.Failure<VentaDetalleDto>(detalleResult.Error);
            }

            detalles.Add(detalleResult.Value);
        }

        var ventaResult = Venta.Create(
            detalles: detalles,
            clienteId: comando.ClienteId.Value,
            stockDisponiblePorProducto: stockPorProducto,
            esAdministrador: comando.EsAdministrador);

        if (ventaResult.IsFailure)
        {
            return Result.Failure<VentaDetalleDto>(ventaResult.Error);
        }

        var nuevoId = await _ventas.AgregarAsync(ventaResult.Value, cancellationToken);

        foreach (var detReq in comando.Detalles)
        {
            var inventario = await _inventarios.ObtenerPorProductoIdAsync(detReq.ProductoId, cancellationToken);
            if (inventario is null) continue;

            var disminuirResult = inventario.Disminuir(detReq.Cantidad);
            if (disminuirResult.IsFailure)
            {
                await _ventas.EliminarAsync(nuevoId, cancellationToken);
                return Result.Failure<VentaDetalleDto>(disminuirResult.Error);
            }

            await _inventarios.ActualizarAsync(inventario, cancellationToken);
        }

        var ventaCreada = await _ventas.ObtenerPorIdAsync(nuevoId, cancellationToken);
        if (ventaCreada is null)
        {
            return Result.Failure<VentaDetalleDto>(VentaErrores.ErrorCreacion);
        }

        return Result.Success(new VentaDetalleDto(
            ventaCreada.Id,
            ventaCreada.FechaVenta,
            ventaCreada.Total,
            ventaCreada.Estado.Value,
            ventaCreada.ClienteId == 0 ? null : ventaCreada.ClienteId));
    }
}
