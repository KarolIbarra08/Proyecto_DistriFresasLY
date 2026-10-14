using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Recepciones;

public class Recepcion : Entity
{
    private readonly List<DetalleRecepcion> _detalles = [];

    public int IdProveedor { get; private set; }
    public DateTime FechaRecepcion { get; private set; }
    public bool ProcesadaEnInventario { get; private set; }


    public double ValorPago => CalcularTotal();

    public IReadOnlyCollection<DetalleRecepcion> Detalles => _detalles.AsReadOnly();

    private Recepcion(
        int id,
        int idProveedor,
        DateTime fechaRecepcion)
        : base(id)
    {
        IdProveedor = idProveedor;
        FechaRecepcion = fechaRecepcion;
        ProcesadaEnInventario = false;
    }

    public static Result<Recepcion> Create(
        int idProveedor,
        int id = 0,
        DateTime? fechaRecepcion = null)
    {
        if (idProveedor <= 0)
        {
            return Result.Failure<Recepcion>(
                Error.Validation("Recepcion.ProveedorInvalido", "El proveedor es obligatorio."));
        }

        return Result.Success(
            new Recepcion(
                id,
                idProveedor,
                fechaRecepcion ?? DateTime.UtcNow));
    }

    public Result Actualizar(int idProveedor)
    {
        if (ProcesadaEnInventario)
        {
            return Result.Failure(
                Error.Conflict("Recepcion.YaProcesada", "No se puede modificar una recepción confirmada en inventario."));
        }

        if (idProveedor <= 0)
        {
            return Result.Failure(
                Error.Validation("Recepcion.ProveedorInvalido", "El proveedor es obligatorio."));
        }

        IdProveedor = idProveedor;

        return Result.Success();
    }

    public Result AgregarDetalle(DetalleRecepcion detalle)
    {
        if (ProcesadaEnInventario)
        {
            return Result.Failure(
                Error.Conflict("Recepcion.YaProcesada", "No se pueden agregar productos a una recepción confirmada."));
        }

        if (detalle is null)
        {
            return Result.Failure(
                Error.Validation("Recepcion.DetalleNulo", "El detalle a agregar no es válido."));
        }

        detalle.AsignarRecepcion(Id);
        _detalles.Add(detalle);

        return Result.Success();
    }

    public Result RemoverDetalle(int idProducto)
    {
        if (ProcesadaEnInventario)
        {
            return Result.Failure(
                Error.Conflict("Recepcion.YaProcesada", "No se pueden eliminar productos de una recepción confirmada."));
        }

        var detalle = _detalles.FirstOrDefault(d => d.IdProducto == idProducto);
        if (detalle is not null)
        {
            _detalles.Remove(detalle);
        }

        return Result.Success();
    }

    public double CalcularTotal()
    {
        return _detalles.Sum(d => d.Subtotal);
    }

    public Result MarcarComoProcesada()
    {
        if (ProcesadaEnInventario)
        {
            return Result.Failure(
                Error.Conflict("Recepcion.YaProcesada", "La recepción ya ha sido procesada previamente en el inventario."));
        }

        if (!_detalles.Any())
        {
            return Result.Failure(
                Error.Validation("Recepcion.SinDetalles", "No se puede confirmar una recepción sin productos agregados."));
        }

        ProcesadaEnInventario = true;
        return Result.Success();
    }
}