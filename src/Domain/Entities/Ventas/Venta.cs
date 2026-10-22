using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Ventas;

public sealed class Venta : Entity
{
    private readonly List<DetalleVenta> _detalles = [];

    public DateTime FechaVenta { get; private set; }
    public EstadoVenta Estado { get; private set; }
    public int ClienteId { get; private set; }
    public IReadOnlyCollection<DetalleVenta> Detalles => _detalles.AsReadOnly();
    public double Total => _detalles.Sum(d => d.Subtotal);

    private Venta(
        int id,
        DateTime fechaVenta,
        EstadoVenta estado,
        int clienteId,
        List<DetalleVenta> detalles) : base(id)
    {
        FechaVenta = fechaVenta;
        Estado = estado;
        ClienteId = clienteId;
        _detalles = detalles;
    }

    public static Result<Venta> Create(
        List<DetalleVenta> detalles,
        int clienteId,
        Dictionary<int, int> stockDisponiblePorProducto,
        bool esAdministrador,
        int id = 0)
    {
        //  Cliente obligatorio
        if (clienteId <= 0)
        {
            return Error.Validation(
                "Venta.ClienteNoRegistrado",
                "El cliente no está registrado en el sistema.");
        }

        if (detalles is null || detalles.Count == 0)
        {
            return Error.Validation(
                "Venta.SinDetalles",
                "La venta debe contener al menos un producto en el detalle.");
        }


        foreach (var detalle in detalles)
        {
            int stockDisponible = stockDisponiblePorProducto.TryGetValue(detalle.ProductoId, out var stock) ? stock : 0;
            int cantidadSolicitada = detalle.Cantidad.Value;

            //  No superar el inventario disponible
            if (cantidadSolicitada > stockDisponible)
            {
                return Error.Validation(
                    "Venta.StockInsuficiente",
                    $"No hay stock suficiente para el producto ID {detalle.ProductoId}. Disponible: {stockDisponible}, solicitado: {cantidadSolicitada}.");
            }

            //  Cantidades mínimas y excepciones
            if (cantidadSolicitada <= 10)
            {
                return Error.Validation(
                    "Venta.CantidadInvalida",
                    "No es posible registrar ventas de 10 libras o menos.");
            }

            if (cantidadSolicitada < 17)
            {
                // Riesgo de pérdida por perecedero (stock total inferior a 17 libras)
                bool riesgoPerdida = stockDisponible < 17;

                if (!riesgoPerdida || !esAdministrador)
                {
                    return Error.Validation(
                        "Venta.CantidadInsuficiente",
                        "No es posible registrar la venta. La cantidad solicitada debe ser mayor a 17 libras.");
                }
            }
        }

        var venta = new Venta(
            id,
            ColombiaTime.Ahora,
            EstadoVenta.Pendiente,
            clienteId,
            detalles);

        return venta;
    }

    public Result ActualizarEstado(string nuevoEstado)
    {
        var estadoResult = EstadoVenta.Create(nuevoEstado);
        if (estadoResult.IsFailure)
        {
            return Result.Failure(estadoResult.Error);
        }

        if (Estado == EstadoVenta.Cancelada)
        {
            var error = Error.Validation(
                "Venta.EstadoInvalido",
                "No se puede cambiar el estado de una venta que ya esta cancelada.");

            return Result.Failure(error);
        }

        Estado = estadoResult.Value;
        return Result.Success();
    }

    public Result ActualizarDetalles(List<DetalleVenta> nuevosDetalles)
    {
        if (Estado == EstadoVenta.Completada || Estado == EstadoVenta.Cancelada)
        {
            var error = Error.Validation(
                "Venta.EdicionNoPermitida",
                $"No se pueden modificar los detalles de una venta en estado '{Estado.Value}'.");

            return Result.Failure(error);
        }

        if (nuevosDetalles is null || nuevosDetalles.Count == 0)
        {
            var error = Error.Validation(
                "Venta.SinDetalles",
                "La venta debe conservar al menos un producto en el detalle.");

            return Result.Failure(error);
        }

        _detalles.Clear();
        _detalles.AddRange(nuevosDetalles);

        return Result.Success();
    }

    public Result AsignarCliente(int nuevoClienteId)
    {
        if (nuevoClienteId <= 0)
        {
            return Result.Failure(
                Error.Validation(
                    "Venta.ClienteNoRegistrado",
                    "El cliente no está registrado en el sistema."));
        }

        ClienteId = nuevoClienteId;
        return Result.Success();
    }
}