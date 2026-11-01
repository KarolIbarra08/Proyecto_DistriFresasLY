using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Pagos;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class PagoClienteRepository : IPagoClienteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PagoClienteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagoCliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_pago_cliente AS Id_Pago_Cliente,
                valor_total AS Valor_Total,
                medio_pago AS Medio_Pago,
                fecha_pago AS Fecha_Pago,
                estado AS Estado,
                id_cliente AS Id_Cliente,
                id_venta AS Id_Venta
            FROM PagoCliente
            WHERE id_pago_cliente = @Id;
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<PagoClienteRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<IReadOnlyList<PagoCliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_pago_cliente AS Id_Pago_Cliente,
                valor_total AS Valor_Total,
                medio_pago AS Medio_Pago,
                fecha_pago AS Fecha_Pago,
                estado AS Estado,
                id_cliente AS Id_Cliente,
                id_venta AS Id_Venta
            FROM PagoCliente;
            """;

        var registros = await conexion.QueryAsync<PagoClienteRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(PagoCliente pagoCliente, CancellationToken cancellationToken = default)
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
                VALUES ('PagoCliente', (SELECT COALESCE(MAX(id_pago_cliente), 0) FROM PagoCliente))
                ON CONFLICT(nombre_entidad) DO UPDATE SET
                    ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_pago_cliente), 0) FROM PagoCliente));
                """;
            await conexion.ExecuteAsync(new CommandDefinition(sqlSinc, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlNextId = """
                UPDATE SecuenciaEntidades 
                SET ultimo_valor = ultimo_valor + 1 
                WHERE nombre_entidad = 'PagoCliente';

                SELECT ultimo_valor 
                FROM SecuenciaEntidades 
                WHERE nombre_entidad = 'PagoCliente';
                """;
            var nuevoId = await conexion.ExecuteScalarAsync<int>(new CommandDefinition(sqlNextId, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlInsert = """
                INSERT INTO PagoCliente (id_pago_cliente, valor_total, medio_pago, fecha_pago, estado, id_cliente, id_venta)
                VALUES (@IdPagoCliente, @ValorTotal, @MedioPago, @FechaPago, @Estado, @IdCliente, @IdVenta);
                """;

            await conexion.ExecuteAsync(new CommandDefinition(sqlInsert, new
            {
                IdPagoCliente = nuevoId,
                ValorTotal = pagoCliente.Valor,
                MedioPago = pagoCliente.MedioPago,
                FechaPago = pagoCliente.FechaPago.ToString("yyyy-MM-dd HH:mm:ss"),
                Estado = pagoCliente.Estado,
                IdCliente = pagoCliente.ClienteId,
                IdVenta = pagoCliente.VentaId
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

    public async Task ActualizarAsync(PagoCliente pagoCliente, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            UPDATE PagoCliente 
            SET valor_total = @ValorTotal,
                medio_pago = @MedioPago,
                fecha_pago = @FechaPago,
                estado = @Estado,
                id_cliente = @IdCliente,
                id_venta = @IdVenta
            WHERE id_pago_cliente = @Id;
            """;

        await conexion.ExecuteAsync(new CommandDefinition(sql, new
        {
            pagoCliente.Id,
            ValorTotal = pagoCliente.Valor,
            MedioPago = pagoCliente.MedioPago,
            FechaPago = pagoCliente.FechaPago.ToString("yyyy-MM-dd HH:mm:ss"),
            Estado = pagoCliente.Estado,
            IdCliente = pagoCliente.ClienteId,
            IdVenta = pagoCliente.VentaId
        }, cancellationToken: cancellationToken));
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = "DELETE FROM PagoCliente WHERE id_pago_cliente = @Id;";
        await conexion.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
