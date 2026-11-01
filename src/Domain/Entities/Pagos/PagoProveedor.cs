using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Pagos;

public sealed class PagoProveedor : Entity
{
    public int ProveedorId { get; private set; }

    public int PedidoId { get; private set; }

    public double Valor { get; private set; }

    public string MedioPago { get; private set; }

    public DateTime FechaPago { get; private set; }

    public string Estado { get; private set; }

    private PagoProveedor(
        int id,
        int proveedorId,
        int pedidoId,
        double valor,
        string medioPago,
        DateTime fechaPago,
        string estado) : base(id)
    {
        ProveedorId = proveedorId;
        PedidoId = pedidoId;
        Valor = valor;
        MedioPago = medioPago;
        FechaPago = fechaPago;
        Estado = estado;
    }

    public static Result<PagoProveedor> Create(
        int proveedorId,
        int pedidoId,
        double valor,
        string medioPago,
        DateTime? fechaPago,
        string estado,
        int id = 0)
    {
        if (proveedorId <= 0)
        {
            return Result.Failure<PagoProveedor>(
                Error.Validation(
                    "PagoProveedor.ProveedorIdInvalido",
                    "El ID del proveedor es obligatorio."));
        }

        if (pedidoId <= 0)
        {
            return Result.Failure<PagoProveedor>(
                Error.Validation(
                    "PagoProveedor.PedidoIdInvalido",
                    "El ID del pedido es obligatorio."));
        }

        if (valor <= 0)
        {
            return Result.Failure<PagoProveedor>(
                Error.Validation(
                    "PagoProveedor.ValorInvalido",
                    "El valor del pago debe ser mayor a cero."));
        }

        if (string.IsNullOrWhiteSpace(medioPago))
        {
            return Result.Failure<PagoProveedor>(
                Error.Validation(
                    "PagoProveedor.MedioPagoObligatorio",
                    "El medio de pago es obligatorio."));
        }

        if (string.IsNullOrWhiteSpace(estado))
        {
            return Result.Failure<PagoProveedor>(
                Error.Validation(
                    "PagoProveedor.EstadoObligatorio",
                    "El estado del pago es obligatorio."));
        }

        DateTime fechaFinal = (!fechaPago.HasValue || fechaPago.Value.Year < 2000)
            ? ColombiaTime.Ahora
            : fechaPago.Value;

        return Result.Success(
            new PagoProveedor(
                id,
                proveedorId,
                pedidoId,
                valor,
                medioPago.Trim(),
                fechaFinal,
                estado.Trim()));
    }

    public static PagoProveedor Rehidratar(
        int id,
        int proveedorId,
        int pedidoId,
        double valor,
        string medioPago,
        DateTime fechaPago,
        string estado)
    {
        return new PagoProveedor(
            id,
            proveedorId,
            pedidoId,
            valor,
            medioPago ?? string.Empty,
            fechaPago,
            estado ?? string.Empty);
    }

    public Result Actualizar(
        int? proveedorId,
        int? pedidoId,
        double? valor,
        string? medioPago,
        DateTime? fechaPago,
        string? estado)
    {
        if (proveedorId.HasValue && proveedorId.Value <= 0)
            return Result.Failure(Error.Validation("PagoProveedor.ProveedorIdInvalido", "ID de proveedor inválido."));

        if (pedidoId.HasValue && pedidoId.Value <= 0)
            return Result.Failure(Error.Validation("PagoProveedor.PedidoIdInvalido", "ID de pedido inválido."));

        if (valor.HasValue && valor.Value <= 0)
            return Result.Failure(Error.Validation("PagoProveedor.ValorInvalido", "El valor debe ser mayor a cero."));

        if (medioPago is not null && string.IsNullOrWhiteSpace(medioPago))
            return Result.Failure(Error.Validation("PagoProveedor.MedioPagoObligatorio", "El medio de pago no puede estar vacío."));

        if (estado is not null && string.IsNullOrWhiteSpace(estado))
            return Result.Failure(Error.Validation("PagoProveedor.EstadoObligatorio", "El estado no puede estar vacío."));

        if (proveedorId.HasValue) ProveedorId = proveedorId.Value;
        if (pedidoId.HasValue) PedidoId = pedidoId.Value;
        if (valor.HasValue) Valor = valor.Value;
        if (!string.IsNullOrWhiteSpace(medioPago)) MedioPago = medioPago.Trim();
        if (fechaPago.HasValue && fechaPago.Value.Year >= 2000) FechaPago = fechaPago.Value;
        if (!string.IsNullOrWhiteSpace(estado)) Estado = estado.Trim();

        return Result.Success();
    }
}
