using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Pedidos;

public sealed class DetallePedido : Entity
{
    public int PedidoId { get; private set; }

    public string Descripcion { get; private set; }

    public Cantidad Cantidad { get; private set; }

    public decimal ValorUnitario { get; private set; }

    public decimal Subtotal => Cantidad * ValorUnitario;

    private DetallePedido(
        int id,
        int pedidoId,
        string descripcion,
        Cantidad cantidad,
        decimal valorUnitario) : base(id)
    {
        PedidoId = pedidoId;
        Descripcion = descripcion;
        Cantidad = cantidad;
        ValorUnitario = valorUnitario;
    }

    public static Result<DetallePedido> Create(
        int pedidoId,
        string descripcion,
        int cantidad,
        decimal valorUnitario,
        int id = 0)
    {
        var cantidadResult = Cantidad.Create(cantidad);
        if (cantidadResult.IsFailure)
        {
            return Result.Failure<DetallePedido>(cantidadResult.Error);
        }

        if (valorUnitario <= 0)
        {
            return Result.Failure<DetallePedido>(
                Error.Validation(
                    "DetallePedido.ValorInvalido",
                    "El valor unitario debe ser mayor a 0."));
        }

        return Result.Success(
            new DetallePedido(
                id,
                pedidoId,
                descripcion?.Trim() ?? string.Empty,
                cantidadResult.Value,
                valorUnitario));
    }
}