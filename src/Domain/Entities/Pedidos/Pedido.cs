using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Pedidos;

public sealed class Pedido : Entity
{
    private readonly List<DetallePedido> _detalles = [];

    public DateTime FechaPedido { get; private set; }

    public string Estado { get; private set; }

    public IReadOnlyCollection<DetallePedido> Detalles => _detalles.AsReadOnly();

    public decimal Total => _detalles.Sum(d => d.Subtotal);

    private Pedido(
        int id,
        DateTime fechaPedido,
        string estado,
        List<DetallePedido> detalles) : base(id)
    {
        FechaPedido = fechaPedido;
        Estado = estado;
        _detalles = detalles;
    }

    public static Result<Pedido> Create(
        List<DetallePedido> detalles,
        DateTime fechaPedido,
        string estado,
        int id = 0)
    {
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