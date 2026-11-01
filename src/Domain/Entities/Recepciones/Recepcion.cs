using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Recepciones;

public class Recepcion : Entity
{
    private readonly List<DetalleRecepcion> _detalles = [];
    public int? PedidoId { get; private set; }
    public DateTime FechaRecepcion { get; private set; }
    public bool ProcesadaEnInventario { get; private set; }


    public double ValorPago => CalcularTotal();

    public IReadOnlyCollection<DetalleRecepcion> Detalles => _detalles.AsReadOnly();

    private Recepcion(
        int id,
        int? pedidoId,
        DateTime fechaRecepcion)
        : base(id)
    {
        PedidoId = pedidoId;
        FechaRecepcion = fechaRecepcion;
        ProcesadaEnInventario = false;
    }

    public static Result<Recepcion> Create(
        int id = 0,
        DateTime? fechaRecepcion = null,
        int? pedidoId = null)
    {
        return Result.Success(
            new Recepcion(
                id,
                pedidoId,
                fechaRecepcion ?? ColombiaTime.Ahora));
    }

    public static Recepcion Rehidratar(
        int id,
        int? pedidoId,
        DateTime fechaRecepcion,
        bool procesadaEnInventario,
        IEnumerable<DetalleRecepcion>? detalles = null)
    {
        var recepcion = new Recepcion(id, pedidoId, fechaRecepcion);

        if (detalles is not null)
        {
            foreach (var detalle in detalles)
            {
                detalle.AsignarRecepcion(id);
                recepcion._detalles.Add(detalle);
            }
        }

        recepcion.ProcesadaEnInventario = procesadaEnInventario;

        return recepcion;
    }

    public Result Actualizar()
    {
        if (ProcesadaEnInventario)
        {
            return Result.Failure(
                Error.Conflict("Recepcion.YaProcesada", "No se puede modificar una recepcion confirmada en inventario."));
        }

        return Result.Success();
    }

    public Result AgregarDetalle(DetalleRecepcion detalle)
    {
        if (ProcesadaEnInventario)
        {
            return Result.Failure(
                Error.Conflict("Recepcion.YaProcesada", "No se pueden agregar productos a una recepcion confirmada."));
        }

        if (detalle is null)
        {
            return Result.Failure(
                Error.Validation("Recepcion.DetalleNulo", "El detalle a agregar no es valido."));
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
                Error.Conflict("Recepcion.YaProcesada", "No se pueden eliminar productos de una recepcion confirmada."));
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
                Error.Conflict("Recepcion.YaProcesada", "La recepcion ya ha sido procesada previamente en el inventario."));
        }

        if (!_detalles.Any())
        {
            return Result.Failure(
                Error.Validation("Recepcion.SinDetalles", "No se puede confirmar una recepcion sin productos agregados."));
        }

        ProcesadaEnInventario = true;
        return Result.Success();
    }
}