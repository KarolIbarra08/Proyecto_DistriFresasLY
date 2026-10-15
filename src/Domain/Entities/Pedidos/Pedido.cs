using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Pedidos;

public sealed class Pedido : Entity
{
    private readonly List<DetallePedido> _detalles = [];

    public int ProveedorId { get; private set; }
    public DateTime FechaPedido { get; private set; }
    public string Estado { get; private set; }
    public IReadOnlyCollection<DetallePedido> Detalles => _detalles.AsReadOnly();
    public double Total => _detalles.Sum(d => d.Subtotal);

    private Pedido(
        int id,
        int proveedorId,
        DateTime fechaPedido,
        string estado,
        List<DetallePedido> detalles) : base(id)
    {
        ProveedorId = proveedorId;
        FechaPedido = fechaPedido;
        Estado = estado;
        _detalles = detalles;
    }

    public static Result<Pedido> Create(
        int proveedorId,
        List<DetallePedido> detalles,
        DateTime fechaPedido,
        string estado,
        int id = 0)
    {
        if (proveedorId <= 0)
        {
            return Result.Failure<Pedido>(
                Error.Validation(
                    "Pedido.ProveedorInvalido",
                    "El ID del proveedor es obligatorio."));
        }

        if (detalles is null || detalles.Count == 0)
        {
            return Result.Failure<Pedido>(
                Error.Validation(
                    "Pedido.SinDetalles",
                    "El pedido debe contener al menos un producto en el detalle."));
        }

        if (string.IsNullOrWhiteSpace(estado))
        {
            return Result.Failure<Pedido>(
                Error.Validation(
                    "Pedido.EstadoObligatorio",
                    "El estado del pedido es obligatorio."));
        }

        var pedido = new Pedido(
            id,
            proveedorId,
            fechaPedido,
            estado.Trim(),
            detalles);

        return Result.Success(pedido);
    }

    public Result ActualizarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
        {
            return Result.Failure(
                Error.Validation(
                    "Pedido.EstadoObligatorio",
                    "El estado del pedido es obligatorio."));
        }

        Estado = nuevoEstado.Trim();

        return Result.Success();
    }

    public Result ActualizarProveedor(int nuevoProveedorId)
    {
        if (nuevoProveedorId <= 0)
        {
            return Result.Failure(
                Error.Validation(
                    "Pedido.ProveedorInvalido",
                    "El ID del proveedor es obligatorio."));
        }

        ProveedorId = nuevoProveedorId;

        return Result.Success();
    }

    public Result ActualizarDetalles(List<DetallePedido> nuevosDetalles)
    {
        if (nuevosDetalles is null || nuevosDetalles.Count == 0)
        {
            return Result.Failure(
                Error.Validation(
                    "Pedido.SinDetalles",
                    "El pedido debe conservar al menos un producto en el detalle."));
        }

        _detalles.Clear();
        _detalles.AddRange(nuevosDetalles);

        return Result.Success();
    }
}
