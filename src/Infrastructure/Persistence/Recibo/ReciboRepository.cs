using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Recibos;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class ReciboRepository : IReciboRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReciboRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Recibo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_recibo AS Id_Recibo,
                fecha AS Fecha,
                numero_recibo AS Numero_Recibo,
                total AS Total,
                id_venta AS Id_Venta
            FROM Recibo
            WHERE id_recibo = @Id;
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<ReciboRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<IReadOnlyList<Recibo>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_recibo AS Id_Recibo,
                fecha AS Fecha,
                numero_recibo AS Numero_Recibo,
                total AS Total,
                id_venta AS Id_Venta
            FROM Recibo;
            """;

        var registros = await conexion.QueryAsync<ReciboRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(Recibo recibo, CancellationToken cancellationToken = default)
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
                VALUES ('Recibo', (SELECT COALESCE(MAX(id_recibo), 0) FROM Recibo))
                ON CONFLICT(nombre_entidad) DO UPDATE SET
                    ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_recibo), 0) FROM Recibo));
                """;
            await conexion.ExecuteAsync(new CommandDefinition(sqlSinc, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlNextId = """
                UPDATE SecuenciaEntidades 
                SET ultimo_valor = ultimo_valor + 1 
                WHERE nombre_entidad = 'Recibo';

                SELECT ultimo_valor 
                FROM SecuenciaEntidades 
                WHERE nombre_entidad = 'Recibo';
                """;
            var nuevoId = await conexion.ExecuteScalarAsync<int>(new CommandDefinition(sqlNextId, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlInsert = """
                INSERT INTO Recibo (id_recibo, fecha, numero_recibo, total, id_venta)
                VALUES (@IdRecibo, @Fecha, @NumeroRecibo, @Total, @IdVenta);
                """;

            await conexion.ExecuteAsync(new CommandDefinition(sqlInsert, new
            {
                IdRecibo = nuevoId,
                Fecha = recibo.FechaRecibo.ToString("yyyy-MM-dd HH:mm:ss"),
                NumeroRecibo = recibo.NumeroRecibo,
                Total = recibo.Total,
                IdVenta = recibo.VentaId
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

    public async Task ActualizarAsync(Recibo recibo, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            UPDATE Recibo 
            SET fecha = @Fecha,
                numero_recibo = @NumeroRecibo,
                total = @Total,
                id_venta = @IdVenta
            WHERE id_recibo = @Id;
            """;

        await conexion.ExecuteAsync(new CommandDefinition(sql, new
        {
            recibo.Id,
            Fecha = recibo.FechaRecibo.ToString("yyyy-MM-dd HH:mm:ss"),
            NumeroRecibo = recibo.NumeroRecibo,
            Total = recibo.Total,
            IdVenta = recibo.VentaId
        }, cancellationToken: cancellationToken));
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = "DELETE FROM Recibo WHERE id_recibo = @Id;";
        await conexion.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
