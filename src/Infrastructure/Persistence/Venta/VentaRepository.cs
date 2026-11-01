using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Ventas;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public VentaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Venta?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sqlVenta = """
            SELECT 
                id_venta AS Id_Venta,
                fecha_venta AS Fecha_Venta,
                total AS Total,
                estado AS Estado,
                id_cliente AS Id_Cliente
            FROM Venta
            WHERE id_venta = @Id;
            """;

        const string sqlDetalles = """
            SELECT 
                id_detalle_venta AS Id_Detalle_Venta,
                cantidad AS Cantidad,
                precio_unitario AS Precio_Unitario,
                subtotal AS Subtotal,
                id_producto AS Id_Producto,
                id_venta AS Id_Venta
            FROM DetalleVenta
            WHERE id_venta = @Id;
            """;

        var ventaRegistro = await conexion.QueryFirstOrDefaultAsync<VentaRegistro>(
            new CommandDefinition(sqlVenta, new { Id = id }, cancellationToken: cancellationToken));

        if (ventaRegistro is null) return null;

        var detalles = await conexion.QueryAsync<DetalleVentaRegistro>(
            new CommandDefinition(sqlDetalles, new { Id = id }, cancellationToken: cancellationToken));

        ventaRegistro.Detalles = detalles.ToList();

        return ventaRegistro.ToEntidad();
    }

    public async Task<IReadOnlyList<Venta>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                v.id_venta AS Id_Venta,
                v.fecha_venta AS Fecha_Venta,
                v.total AS Total,
                v.estado AS Estado,
                v.id_cliente AS Id_Cliente,
                d.id_detalle_venta AS Id_Detalle_Venta,
                d.cantidad AS Cantidad,
                d.precio_unitario AS Precio_Unitario,
                d.subtotal AS Subtotal,
                d.id_producto AS Id_Producto,
                d.id_venta AS Id_Venta_Detalle
            FROM Venta v
            LEFT JOIN DetalleVenta d ON v.id_venta = d.id_venta;
            """;

        var ventaDictionary = new Dictionary<int, VentaRegistro>();

        await conexion.QueryAsync<VentaRegistro, DetalleVentaRegistro, VentaRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken),
            (venta, detalle) =>
            {
                if (!ventaDictionary.TryGetValue(venta.Id_Venta, out var currentVenta))
                {
                    currentVenta = venta;
                    currentVenta.Detalles = new List<DetalleVentaRegistro>();
                    ventaDictionary.Add(currentVenta.Id_Venta, currentVenta);
                }

                if (detalle is not null && detalle.Id_Detalle_Venta > 0)
                {
                    currentVenta.Detalles.Add(detalle);
                }

                return currentVenta;
            },
            splitOn: "Id_Detalle_Venta");

        return ventaDictionary.Values.Select(v => v.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(Venta venta, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlVenta = """
                INSERT INTO Venta (fecha_venta, total, estado, id_cliente)
                VALUES (@FechaVenta, @Total, @Estado, @IdCliente);

                SELECT last_insert_rowid();
                """;

            var parametrosVenta = new
            {
                FechaVenta = venta.FechaVenta.ToString("yyyy-MM-dd HH:mm:ss"),
                Total = venta.Total,
                Estado = venta.Estado.Value,
                IdCliente = venta.ClienteId == 0 ? (int?)null : venta.ClienteId
            };

            var ventaId = await conexion.ExecuteScalarAsync<int>(
                new CommandDefinition(sqlVenta, parametrosVenta, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlDetalle = """
                INSERT INTO DetalleVenta (cantidad, precio_unitario, subtotal, id_producto, id_venta)
                VALUES (@Cantidad, @PrecioUnitario, @Subtotal, @IdProducto, @IdVenta);
                """;

            foreach (var detalle in venta.Detalles)
            {
                var parametrosDetalle = new
                {
                    Cantidad = detalle.Cantidad.Value,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Subtotal = detalle.Subtotal,
                    IdProducto = detalle.ProductoId,
                    IdVenta = ventaId
                };

                await conexion.ExecuteAsync(
                    new CommandDefinition(sqlDetalle, parametrosDetalle, transaction: transaccion, cancellationToken: cancellationToken));
            }

            transaccion.Commit();
            return ventaId;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task ActualizarAsync(Venta venta, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlVenta = """
                UPDATE Venta
                SET fecha_venta = @FechaVenta,
                    total = @Total,
                    estado = @Estado,
                    id_cliente = @IdCliente
                WHERE id_venta = @Id;
                """;

            var parametrosVenta = new
            {
                Id = venta.Id,
                FechaVenta = venta.FechaVenta.ToString("yyyy-MM-dd HH:mm:ss"),
                Total = venta.Total,
                Estado = venta.Estado.Value,
                IdCliente = venta.ClienteId == 0 ? (int?)null : venta.ClienteId
            };

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlVenta, parametrosVenta, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlDeleteDetalles = "DELETE FROM DetalleVenta WHERE id_venta = @VentaId;";
            await conexion.ExecuteAsync(
                new CommandDefinition(sqlDeleteDetalles, new { VentaId = venta.Id }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlInsertDetalle = """
                INSERT INTO DetalleVenta (cantidad, precio_unitario, subtotal, id_producto, id_venta)
                VALUES (@Cantidad, @PrecioUnitario, @Subtotal, @IdProducto, @IdVenta);
                """;

            foreach (var detalle in venta.Detalles)
            {
                var parametrosDetalle = new
                {
                    Cantidad = detalle.Cantidad.Value,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Subtotal = detalle.Subtotal,
                    IdProducto = detalle.ProductoId,
                    IdVenta = venta.Id
                };

                await conexion.ExecuteAsync(
                    new CommandDefinition(sqlInsertDetalle, parametrosDetalle, transaction: transaccion, cancellationToken: cancellationToken));
            }

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlDeleteDetalles = "DELETE FROM DetalleVenta WHERE id_venta = @Id;";
            await conexion.ExecuteAsync(
                new CommandDefinition(sqlDeleteDetalles, new { Id = id }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlDeleteVenta = "DELETE FROM Venta WHERE id_venta = @Id;";
            await conexion.ExecuteAsync(
                new CommandDefinition(sqlDeleteVenta, new { Id = id }, transaction: transaccion, cancellationToken: cancellationToken));

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task<bool> TienePagosAsociadosAsync(int idVenta, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = "SELECT COUNT(1) FROM PagoCliente WHERE id_venta = @Id;";

        var cantidad = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Id = idVenta }, cancellationToken: cancellationToken));

        return cantidad > 0;
    }

    public async Task<bool> TieneReciboAsociadoAsync(int idVenta, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = "SELECT COUNT(1) FROM Recibo WHERE id_venta = @Id;";

        var cantidad = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Id = idVenta }, cancellationToken: cancellationToken));

        return cantidad > 0;
    }
}
