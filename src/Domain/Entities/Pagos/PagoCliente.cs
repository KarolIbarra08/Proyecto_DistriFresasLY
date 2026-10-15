using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Pagos;

public sealed class PagoCliente : Entity
{
    public int ClienteId { get; private set; }
    public int VentaId { get; private set; }
    public double Valor { get; private set; }
    public string MedioPago { get; private set; }
    public DateTime FechaPago { get; private set; }
    public string Estado { get; private set; }

    private PagoCliente(
        int id,
        int clienteId,
        int ventaId,
        double valor,
        string medioPago,
        DateTime fechaPago,
        string estado) : base(id)
    {
        ClienteId = clienteId;
        VentaId = ventaId;
        Valor = valor;
        MedioPago = medioPago;
        FechaPago = fechaPago;
        Estado = estado;
    }

    public static Result<PagoCliente> Create(
        int clienteId,
        int ventaId,
        double valor,
        string medioPago,
        DateTime? fechaPago,
        string estado,
        int id = 0)
    {
        if (clienteId <= 0)
        {
            return Result.Failure<PagoCliente>(
                Error.Validation("PagoCliente.ClienteIdInvalido", "El ID del cliente es obligatorio."));
        }

        if (ventaId <= 0)
        {
            return Result.Failure<PagoCliente>(
                Error.Validation("PagoCliente.VentaIdInvalido", "El ID de la venta es obligatorio."));
        }

        if (valor <= 0)
        {
            return Result.Failure<PagoCliente>(
                Error.Validation("PagoCliente.ValorInvalido", "El valor del pago debe ser mayor a cero."));
        }

        if (string.IsNullOrWhiteSpace(medioPago))
        {
            return Result.Failure<PagoCliente>(
                Error.Validation("PagoCliente.MedioPagoObligatorio", "El medio de pago es obligatorio."));
        }

        if (string.IsNullOrWhiteSpace(estado))
        {
            return Result.Failure<PagoCliente>(
                Error.Validation("PagoCliente.EstadoObligatorio", "El estado del pago es obligatorio."));
        }

        DateTime fechaFinal = (!fechaPago.HasValue || fechaPago.Value.Year < 2000)
            ? DateTime.Now
            : fechaPago.Value;

        return Result.Success(
            new PagoCliente(
                id,
                clienteId,
                ventaId,
                (double)valor,
                medioPago.Trim(),
                fechaFinal,
                estado.Trim()));
    }

    public Result Actualizar(
        int? clienteId,
        int? ventaId,
        double? valor,
        string? medioPago,
        DateTime? fechaPago,
        string? estado)
    {
        if (clienteId.HasValue && clienteId.Value <= 0)
            return Result.Failure(Error.Validation("PagoCliente.ClienteIdInvalido", "ID de cliente inválido."));

        if (ventaId.HasValue && ventaId.Value <= 0)
            return Result.Failure(Error.Validation("PagoCliente.VentaIdInvalido", "ID de venta inválido."));

        if (valor.HasValue && valor.Value <= 0)
            return Result.Failure(Error.Validation("PagoCliente.ValorInvalido", "El valor debe ser mayor a cero."));

        if (medioPago is not null && string.IsNullOrWhiteSpace(medioPago))
            return Result.Failure(Error.Validation("PagoCliente.MedioPagoObligatorio", "El medio de pago no puede estar vacío."));

        if (estado is not null && string.IsNullOrWhiteSpace(estado))
            return Result.Failure(Error.Validation("PagoCliente.EstadoObligatorio", "El estado no puede estar vacío."));

        if (clienteId.HasValue) ClienteId = clienteId.Value;
        if (ventaId.HasValue) VentaId = ventaId.Value;
        if (valor.HasValue) Valor = valor.Value;
        if (!string.IsNullOrWhiteSpace(medioPago)) MedioPago = medioPago.Trim();
        if (fechaPago.HasValue && fechaPago.Value.Year >= 2000) FechaPago = fechaPago.Value;
        if (!string.IsNullOrWhiteSpace(estado)) Estado = estado.Trim();

        return Result.Success();
    }
}