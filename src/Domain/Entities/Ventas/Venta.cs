namespace DistriFresasLY.Domain.Entities;

public class Venta
{
    public int Id { get; private set; }
    public DateTime FechaVenta { get; private set; }
    public string Estado { get; private set; }
    public int? ClienteId { get; private set; }

    private readonly List<DetalleVenta> _detalles = [];
    public IReadOnlyCollection<DetalleVenta> Detalles => _detalles.AsReadOnly();

    // Regla de Dominio: El total es calculado por el agregado Venta
    public decimal Total => _detalles.Sum(d => d.Subtotal);

    public Venta(int id, int? clienteId, string estado = "Pendiente")
    {
        Id = id;
        ClienteId = clienteId;
        FechaVenta = DateTime.Now;
        Estado = estado;
    }

    public void AgregarDetalle(int productoId, int cantidad, decimal precioUnitario)
    {
        _detalles.Add(new DetalleVenta(productoId, cantidad, precioUnitario));
    }

    public void ActualizarCliente(int? clienteId)
    {
        if (clienteId.HasValue)
            ClienteId = clienteId.Value;
    }

    public void CambiarEstado(string nuevoEstado)
    {
        if (!string.IsNullOrWhiteSpace(nuevoEstado))
            Estado = nuevoEstado.Trim();
    }

    public void ReemplazarDetalles(List<DetalleVenta> nuevosDetalles)
    {
        _detalles.Clear();
        _detalles.AddRange(nuevosDetalles);
    }
}