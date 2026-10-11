using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Recepciones;

public class Recepcion : Entity
{
    private readonly List<DetalleRecepcion> _detalles = [];

    public DateTime FechaRecepcion { get; private set; }
    public decimal ValorPago { get; private set; }

    public IReadOnlyCollection<DetalleRecepcion> Detalles =>
        _detalles.AsReadOnly();

    private Recepcion(
        int id,
        DateTime fechaRecepcion,
        decimal valorPago)
        : base(id)
    {
        FechaRecepcion = fechaRecepcion;
        ValorPago = valorPago;
    }

    public static Result<Recepcion> Create(
        decimal valorPago,
        int id = 0,
        DateTime? fechaRecepcion = null)
    {
        if (valorPago < 0)
        {
            return Result.Failure<Recepcion>(
                Error.Validation(
                    "Recepcion.ValorPagoInvalido",
                    "El valor del pago no puede ser negativo."));
        }

        return Result.Success(
            new Recepcion(
                id,
                fechaRecepcion ?? DateTime.UtcNow,
                valorPago));
    }

    public Result Actualizar(
        decimal valorPago,
        DateTime? fechaRecepcion = null)
    {
        if (valorPago < 0)
        {
            return Result.Failure(
                Error.Validation(
                    "Recepcion.ValorPagoInvalido",
                    "El valor del pago no puede ser negativo."));
        }

        ValorPago = valorPago;

        if (fechaRecepcion.HasValue)
            FechaRecepcion = fechaRecepcion.Value;

        return Result.Success();
    }

    public void AgregarDetalle(DetalleRecepcion detalle)
    {
        if (detalle is null)
            throw new ArgumentNullException(nameof(detalle));

        _detalles.Add(detalle);
    }
    public decimal CalcularTotal()
    {
        return _detalles.Sum(d => d.Subtotal);
    }
}