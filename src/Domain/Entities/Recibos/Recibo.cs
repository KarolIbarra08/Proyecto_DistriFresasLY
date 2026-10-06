using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Recibos;

public sealed class Recibo : Entity
{
    public DateTime FechaRecibo { get; private set; }
    public int NumeroRecibo { get; private set; }
    public decimal Total { get; private set; }
    public int VentaId { get; private set; }

    private Recibo(int id, DateTime fechaRecibo, int numeroRecibo, decimal total, int ventaId) : base(id)
    {
        FechaRecibo = fechaRecibo;
        NumeroRecibo = numeroRecibo;
        Total = total;
        VentaId = ventaId;
    }

    public static Result<Recibo> Create(int ventaId, decimal total, int ultimoNumeroRecibo, int id = 0)
    {
        if (ventaId <= 0)
        {
            return Result.Failure<Recibo>(Error.Validation(
                "Recibo.VentaInvalida", 
                "El ID de la venta no es valido."));
        }

        if (total <= 0)
        {
            return Result.Failure<Recibo>(Error.Validation(
                "Recibo.TotalInvalido", 
                "El total del recibo debe ser mayor a cero."));
        }

        int consecutivo = ultimoNumeroRecibo > 0 ? ultimoNumeroRecibo + 1 : 1001;

        var recibo = new Recibo(
            id: id,
            fechaRecibo: DateTime.Now,
            numeroRecibo: consecutivo,
            total: total,
            ventaId: ventaId);

        return Result.Success(recibo);
    }
}