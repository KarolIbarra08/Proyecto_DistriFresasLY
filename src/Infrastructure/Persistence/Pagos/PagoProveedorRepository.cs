using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Pagos;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class PagoProveedorRepository : IPagoProveedorRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PagoProveedorRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagoProveedor?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_pago_proveedor AS Id_Pago_Proveedor,
                valor_total AS Valor_Total,
                medio_pago AS Medio_Pago,
                fecha_pago AS Fecha_Pago,
                estado AS Estado,
                id_proveedor AS Id_Proveedor,
                id_pedido AS Id_Pedido
            FROM PagoProveedor
            WHERE id_pago_proveedor = @Id;
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<PagoProveedorRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<IReadOnlyList<PagoProveedor>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_pago_proveedor AS Id_Pago_Proveedor,
                valor_total AS Valor_Total,
                medio_pago AS Medio_Pago,
                fecha_pago AS Fecha_Pago,
                estado AS Estado,
                id_proveedor AS Id_Proveedor,
                id_pedido AS Id_Pedido
            FROM PagoProveedor;
            """;

        var registros = await conexion.QueryAsync<PagoProveedorRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(PagoProveedor pagoProveedor, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlInitSecuencia = """
                CREATE TABLE IF NOT EXISTS SecuenciaEntidades (
                    nombre_entidad TEXT PRIMARY KEY,
                    ultimo_valor INTEGER NOT NULL
                );
                """;
            await conexion.ExecuteAsync(new CommandDefinition(sqlInitSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlSinc = """
                INSERT INTO SecuenciaEntidades (nombre_entidad, ultimo_valor)
                VALUES ('PagoProveedor', (SELECT COALESCE(MAX(id_pago_proveedor), 0) FROM PagoProveedor))
                ON CONFLICT(nombre_entidad) DO UPDATE SET
                    ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_pago_proveedor), 0) FROM PagoProveedor));
                """;
            await conexion.ExecuteAsync(new CommandDefinition(sqlSinc, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlNextId = """
                UPDATE SecuenciaEntidades 
                SET ultimo_valor = ultimo_valor + 1 
                WHERE nombre_entidad = 'PagoProveedor';

                SELECT ultimo_valor 
                FROM SecuenciaEntidades 
                WHERE nombre_entidad = 'PagoProveedor';
                """;
            var nuevoId = await conexion.ExecuteScalarAsync<int>(new CommandDefinition(sqlNextId, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlInsert = """
                INSERT INTO PagoProveedor (id_pago_proveedor, valor_total, medio_pago, fecha_pago, estado, id_proveedor, id_pedido)
                VALUES (@IdPagoProveedor, @ValorTotal, @MedioPago, @FechaPago, @Estado, @IdProveedor, @IdPedido);
                """;

            await conexion.ExecuteAsync(new CommandDefinition(sqlInsert, new
            {
                IdPagoProveedor = nuevoId,
                ValorTotal = pagoProveedor.Valor,
                MedioPago = pagoProveedor.MedioPago,
                FechaPago = pagoProveedor.FechaPago.ToString("yyyy-MM-dd HH:mm:ss"),
                Estado = pagoProveedor.Estado,
                IdProveedor = pagoProveedor.ProveedorId,
                IdPedido = pagoProveedor.PedidoId
            }, transaction: transaccion, cancellationToken: cancellationToken));

            transaccion.Commit();
            return nuevoId;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task ActualizarAsync(PagoProveedor pagoProveedor, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            UPDATE PagoProveedor 
            SET valor_total = @ValorTotal,
                medio_pago = @MedioPago,
                fecha_pago = @FechaPago,
                estado = @Estado,
                id_proveedor = @IdProveedor,
                id_pedido = @IdPedido
            WHERE id_pago_proveedor = @Id;
            """;

        await conexion.ExecuteAsync(new CommandDefinition(sql, new
        {
            pagoProveedor.Id,
            ValorTotal = pagoProveedor.Valor,
            MedioPago = pagoProveedor.MedioPago,
            FechaPago = pagoProveedor.FechaPago.ToString("yyyy-MM-dd HH:mm:ss"),
            Estado = pagoProveedor.Estado,
            IdProveedor = pagoProveedor.ProveedorId,
            IdPedido = pagoProveedor.PedidoId
        }, cancellationToken: cancellationToken));
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = "DELETE FROM PagoProveedor WHERE id_pago_proveedor = @Id;";
        await conexion.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
