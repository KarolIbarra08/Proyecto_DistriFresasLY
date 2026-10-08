using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Recepciones;

public class Recepcion
{
    public int Id { get; private set; }
    public DateTime FechaRecepcion { get; private set; }
    public int Cantidad { get; private set; }
    public double ValorPago { get; private set; }

    private Recepcion(int id, DateTime fechaRecepcion, int cantidad, double valorPago)
    {
        Id = id;
        FechaRecepcion = fechaRecepcion;
        Cantidad = cantidad;
        ValorPago = valorPago;
    }

    public static Result<Recepcion> Create(
        int cantidad, 
        double valorPago, 
        int id = 0, 
        DateTime? fechaRecepcion = null)
    {
        if (cantidad <= 0)
            return Result.Failure<Recepcion>(Error.Validation("Recepcion.CantidadInvalida", "La cantidad debe ser mayor a cero."));

        if (valorPago < 0)
            return Result.Failure<Recepcion>(Error.Validation("Recepcion.ValorInvalido", "El valor del pago no puede ser negativo."));

        return Result.Success(new Recepcion(
            id,
            fechaRecepcion ?? DateTime.UtcNow,
            cantidad,
            valorPago
        ));
    }
}