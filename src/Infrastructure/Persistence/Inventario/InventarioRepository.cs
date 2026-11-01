using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Inventario;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class InventarioRepository : IInventarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public InventarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Inventario?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_inventario AS Id_Inventario,
                id_producto AS Id_Producto,
                cantidad_disponible AS Cantidad_Disponible,
                fecha_actualizacion AS Fecha_Actualizacion
            FROM Inventario
            WHERE id_inventario = @Id;
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<InventarioRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<Inventario?> ObtenerPorProductoIdAsync(int productoId, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_inventario AS Id_Inventario,
                id_producto AS Id_Producto,
                cantidad_disponible AS Cantidad_Disponible,
                fecha_actualizacion AS Fecha_Actualizacion
            FROM Inventario
            WHERE id_producto = @ProductoId;
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<InventarioRegistro>(
            new CommandDefinition(sql, new { ProductoId = productoId }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<IReadOnlyList<Inventario>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_inventario AS Id_Inventario,
                id_producto AS Id_Producto,
                cantidad_disponible AS Cantidad_Disponible,
                fecha_actualizacion AS Fecha_Actualizacion
            FROM Inventario;
            """;

        var registros = await conexion.QueryAsync<InventarioRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(Inventario inventario, CancellationToken cancellationToken = default)
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

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlInitSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlSincronizarSecuencia = """
                INSERT INTO SecuenciaEntidades (nombre_entidad, ultimo_valor)
                VALUES ('Inventario', (SELECT COALESCE(MAX(id_inventario), 0) FROM Inventario))
                ON CONFLICT(nombre_entidad) DO UPDATE SET
                    ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_inventario), 0) FROM Inventario));
                """;

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlSincronizarSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlObtenerSiguienteId = """
                UPDATE SecuenciaEntidades 
                SET ultimo_valor = ultimo_valor + 1 
                WHERE nombre_entidad = 'Inventario';

                SELECT ultimo_valor 
                FROM SecuenciaEntidades 
                WHERE nombre_entidad = 'Inventario';
                """;

            var nuevoId = await conexion.ExecuteScalarAsync<int>(
                new CommandDefinition(sqlObtenerSiguienteId, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlInsert = """
                INSERT INTO Inventario (id_inventario, id_producto, cantidad_disponible, fecha_actualizacion)
                VALUES (@IdInventario, @ProductoId, @CantidadDisponible, @FechaActualizacion);
                """;

            var parametros = new
            {
                IdInventario = nuevoId,
                inventario.ProductoId,
                CantidadDisponible = inventario.CantidadDisponible,
                FechaActualizacion = inventario.FechaActualizacion.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlInsert, parametros, transaction: transaccion, cancellationToken: cancellationToken));

            transaccion.Commit();
            return nuevoId;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task ActualizarAsync(Inventario inventario, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            UPDATE Inventario 
            SET cantidad_disponible = @CantidadDisponible,
                fecha_actualizacion = @FechaActualizacion
            WHERE id_inventario = @Id;
            """;

        var parametros = new
        {
            inventario.Id,
            CantidadDisponible = inventario.CantidadDisponible,
            FechaActualizacion = inventario.FechaActualizacion.ToString("yyyy-MM-dd HH:mm:ss")
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = "DELETE FROM Inventario WHERE id_inventario = @Id;";

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
