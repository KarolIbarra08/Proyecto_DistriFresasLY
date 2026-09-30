using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Ventas;

public sealed class Venta : Entity
{
    private readonly List<DetalleVenta> _detalles = [];

    public DateTime FechaVenta { get; private set; }
    public EstadoVenta Estado { get; private set; }
    public int? ClienteId { get; private set; }
    public IReadOnlyCollection<DetalleVenta> Detalles => _detalles.AsReadOnly();
    public decimal Total => _detalles.Sum(d => d.Subtotal);

    private Venta(
        int id,
        DateTime fechaVenta,
        EstadoVenta estado,
        int? clienteId,
        List<DetalleVenta> detalles) : base(id)
    {
        FechaVenta = fechaVenta;
        Estado = estado;
        ClienteId = clienteId;
        _detalles = detalles;
    }

    public static Result<Venta> Create(
        List<DetalleVenta> detalles,
        int? clienteId = null,
        int id = 0)
    {
        if (detalles is null || detalles.Count == 0)
        {
            return Error.Validation(
                "Venta.SinDetalles",
                "La venta debe contener al menos un producto en el detalle.");
        }

        if (clienteId.HasValue && clienteId.Value <= 0)
        {
            return Error.Validation(
                "Venta.ClienteInvalido",
                "El ID del cliente no es valido.");
        }

        var venta = new Venta(
            id,
            DateTime.Now,
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

    public void AsignarCliente(int? nuevoClienteId)
    {
        ClienteId = nuevoClienteId;
    }
}