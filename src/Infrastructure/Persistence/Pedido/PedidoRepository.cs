using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Pedidos;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PedidoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Pedido?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sqlPedido = """
            SELECT 
                id_pedido AS Id_Pedido,
                fecha_pedido AS Fecha_Pedido,
                estado AS Estado,
                total AS Total,
                id_proveedor AS Id_Proveedor
            FROM Pedidos
            WHERE id_pedido = @Id;
            """;

        const string sqlDetalles = """
            SELECT 
                id_detalle_pedido AS Id_Detalle_Pedido,
                descripcion AS Descripcion,
                cantidad AS Cantidad,
                precio_unitario AS Precio_Unitario,
                id_pedido AS Id_Pedido,
                id_producto AS Id_Producto
            FROM DetallePedido
            WHERE id_pedido = @Id;
            """;

        var pedidoRegistro = await conexion.QueryFirstOrDefaultAsync<PedidoRegistro>(
            new CommandDefinition(sqlPedido, new { Id = id }, cancellationToken: cancellationToken));

        if (pedidoRegistro is null) return null;

        var detalles = await conexion.QueryAsync<DetallePedidoRegistro>(
            new CommandDefinition(sqlDetalles, new { Id = id }, cancellationToken: cancellationToken));

        pedidoRegistro.Detalles = detalles.ToList();

        return pedidoRegistro.ToEntidad();
    }

    public async Task<IReadOnlyList<Pedido>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                p.id_pedido AS Id_Pedido,
                p.fecha_pedido AS Fecha_Pedido,
                p.estado AS Estado,
                p.total AS Total,
                p.id_proveedor AS Id_Proveedor,
                d.id_detalle_pedido AS Id_Detalle_Pedido,
                d.descripcion AS Descripcion,
                d.cantidad AS Cantidad,
                d.precio_unitario AS Precio_Unitario,
                d.id_pedido AS Id_Pedido,
                d.id_producto AS Id_Producto
            FROM Pedidos p
            LEFT JOIN DetallePedido d ON p.id_pedido = d.id_pedido;
            """;

        var pedidoDictionary = new Dictionary<int, PedidoRegistro>();

        await conexion.QueryAsync<PedidoRegistro, DetallePedidoRegistro, PedidoRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken),
            (pedido, detalle) =>
            {
                if (!pedidoDictionary.TryGetValue(pedido.Id_Pedido, out var currentPedido))
                {
                    currentPedido = pedido;
                    currentPedido.Detalles = new List<DetallePedidoRegistro>();
                    pedidoDictionary.Add(currentPedido.Id_Pedido, currentPedido);
                }

                if (detalle is not null && detalle.Id_Detalle_Pedido > 0)
                {
                    currentPedido.Detalles.Add(detalle);
                }

                return currentPedido;
            },
            splitOn: "Id_Detalle_Pedido");

        return pedidoDictionary.Values.Select(p => p.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlPedido = """
                INSERT INTO Pedidos (fecha_pedido, estado, total, id_proveedor)
                VALUES (@FechaPedido, @Estado, @Total, @ProveedorId);

                SELECT last_insert_rowid();
                """;

            var parametrosPedido = new
            {
                FechaPedido = pedido.FechaPedido.ToString("yyyy-MM-dd HH:mm:ss"),
                Estado = pedido.Estado,
                Total = pedido.Total,
                ProveedorId = pedido.ProveedorId
            };

            var pedidoId = await conexion.ExecuteScalarAsync<int>(
                new CommandDefinition(sqlPedido, parametrosPedido, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlDetalle = """
                INSERT INTO DetallePedido (descripcion, cantidad, precio_unitario, id_pedido, id_producto)
                VALUES (@Descripcion, @Cantidad, @PrecioUnitario, @PedidoId, @ProductoId);
                """;

            foreach (var detalle in pedido.Detalles)
            {
                var parametrosDetalle = new
                {
                    Descripcion = detalle.Descripcion,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.ValorUnitario,
                    PedidoId = pedidoId,
                    ProductoId = 1 
                };

                await conexion.ExecuteAsync(
                    new CommandDefinition(sqlDetalle, parametrosDetalle, transaction: transaccion, cancellationToken: cancellationToken));
            }

            transaccion.Commit();
            return pedidoId;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task ActualizarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlPedido = """
                UPDATE Pedidos
                SET fecha_pedido = @FechaPedido,
                    estado = @Estado,
                    total = @Total,
                    id_proveedor = @ProveedorId
                WHERE id_pedido = @Id;
                """;

            var parametrosPedido = new
            {
                Id = pedido.Id,
                FechaPedido = pedido.FechaPedido.ToString("yyyy-MM-dd HH:mm:ss"),
                Estado = pedido.Estado,
                Total = pedido.Total,
                ProveedorId = pedido.ProveedorId
            };

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlPedido, parametrosPedido, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlDeleteDetalles = "DELETE FROM DetallePedido WHERE id_pedido = @PedidoId;";
            await conexion.ExecuteAsync(
                new CommandDefinition(sqlDeleteDetalles, new { PedidoId = pedido.Id }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlInsertDetalle = """
                INSERT INTO DetallePedido (descripcion, cantidad, precio_unitario, id_pedido, id_producto)
                VALUES (@Descripcion, @Cantidad, @PrecioUnitario, @PedidoId, @ProductoId);
                """;

            foreach (var detalle in pedido.Detalles)
            {
                var parametrosDetalle = new
                {
                    Descripcion = detalle.Descripcion,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.ValorUnitario,
                    PedidoId = pedido.Id,
                    ProductoId = 1
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

        const string sql = "DELETE FROM Pedidos WHERE id_pedido = @Id;";

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}