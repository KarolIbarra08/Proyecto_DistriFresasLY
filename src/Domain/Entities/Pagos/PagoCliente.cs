using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Pagos;

public sealed class PagoCliente : Entity
{
    public int ClienteId { get; private set; }

    public decimal Valor { get; private set; }

    public string MedioPago { get; private set; }

    public DateTime FechaInicial { get; private set; }

    public DateTime FechaFinal { get; private set; }

    public string Estado { get; private set; }

    private PagoCliente(
        int id,
        int clienteId,
        decimal valor,
        string medioPago,
        DateTime fechaInicial,
        DateTime fechaFinal,
        string estado) : base(id)
    {
        ClienteId = clienteId;
        Valor = valor;
        MedioPago = medioPago;
        FechaInicial = fechaInicial;
        FechaFinal = fechaFinal;
        Estado = estado;
    }

    public static Result<PagoCliente> Create(
        int clienteId,
        decimal valor,
        string medioPago,
        DateTime fechaInicial,
        DateTime fechaFinal,
        string estado,
        int id = 0)
    {
        if (valor <= 0)
        {
            return Result.Failure<PagoCliente>(
                Error.Validation(
                    "PagoCliente.ValorInvalido",
                    "El valor del pago debe ser mayor a cero."));
        }

        if (string.IsNullOrWhiteSpace(estado))
        {
            return Result.Failure<PagoCliente>(
                Error.Validation(
                    "PagoCliente.EstadoObligatorio",
                    "El estado del pago es obligatorio."));
        }

        return Result.Success(
            new PagoCliente(
                id,
                clienteId,
                valor,
                medioPago?.Trim() ?? string.Empty,
                fechaInicial,
                fechaFinal,
                estado.Trim()));
    }

    public Result Actualizar(
        int? clienteId,
        decimal? valor,
        string? medioPago,
        DateTime? fechaInicial,
        DateTime? fechaFinal,
        string? estado)
    {
        if (valor.HasValue && valor.Value <= 0)
        {
            return Result.Failure(
                Error.Validation(
                    "PagoCliente.ValorInvalido",
                    "El valor del pago debe ser mayor a cero."));
        }

        if (estado is not null && string.IsNullOrWhiteSpace(estado))
        {
            return Result.Failure(
                Error.Validation(
                    "PagoCliente.EstadoObligatorio",
                    "El estado del pago es obligatorio."));
        }

        if (clienteId.HasValue) ClienteId = clienteId.Value;
        if (valor.HasValue) Valor = valor.Value;
        if (!string.IsNullOrWhiteSpace(medioPago)) MedioPago = medioPago.Trim();
        if (fechaInicial.HasValue) FechaInicial = fechaInicial.Value;
        if (fechaFinal.HasValue) FechaFinal = fechaFinal.Value;
        if (!string.IsNullOrWhiteSpace(estado)) Estado = estado.Trim();

        return Result.Success();
    }
}