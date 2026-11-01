using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Recepciones;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class RecepcionRepository : IRecepcionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RecepcionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Recepcion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sqlRecepcion = """
            SELECT 
                id_recepcion AS Id_Recepcion,
                fecha_recepcion AS Fecha_Recepcion,
                id_pedido AS Id_Pedido,
                procesada_en_inventario AS Procesada_En_Inventario
            FROM Recepciones
            WHERE id_recepcion = @Id;
            """;

        const string sqlDetalles = """
            SELECT 
                id_detalle_recepcion AS Id_Detalle_Recepcion,
                cantidad AS Cantidad,
                precio_unitario AS Precio_Unitario,
                fecha_ingreso AS Fecha_Ingreso,
                id_recepcion AS Id_Recepcion,
                id_producto AS Id_Producto
            FROM DetalleRecepcion
            WHERE id_recepcion = @Id;
            """;

        var recepcionRegistro = await conexion.QueryFirstOrDefaultAsync<RecepcionRegistro>(
            new CommandDefinition(sqlRecepcion, new { Id = id }, cancellationToken: cancellationToken));

        if (recepcionRegistro is null) return null;

        var detalles = await conexion.QueryAsync<DetalleRecepcionRegistro>(
            new CommandDefinition(sqlDetalles, new { Id = id }, cancellationToken: cancellationToken));

        recepcionRegistro.Detalles = detalles.ToList();

        return recepcionRegistro.ToEntidad();
    }

    public async Task<IReadOnlyList<Recepcion>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sqlRecepciones = """
            SELECT 
                id_recepcion AS Id_Recepcion,
                fecha_recepcion AS Fecha_Recepcion,
                id_pedido AS Id_Pedido,
                procesada_en_inventario AS Procesada_En_Inventario
            FROM Recepciones;
            """;

        const string sqlDetalles = """
            SELECT 
                id_detalle_recepcion AS Id_Detalle_Recepcion,
                cantidad AS Cantidad,
                precio_unitario AS Precio_Unitario,
                fecha_ingreso AS Fecha_Ingreso,
                id_recepcion AS Id_Recepcion,
                id_producto AS Id_Producto
            FROM DetalleRecepcion;
            """;

        var recepciones = (await conexion.QueryAsync<RecepcionRegistro>(
            new CommandDefinition(sqlRecepciones, cancellationToken: cancellationToken))).ToList();

        if (recepciones.Count == 0) return [];

        var detalles = (await conexion.QueryAsync<DetalleRecepcionRegistro>(
            new CommandDefinition(sqlDetalles, cancellationToken: cancellationToken))).GroupBy(d => d.Id_Recepcion);

        foreach (var recepcion in recepciones)
        {
            var dets = detalles.FirstOrDefault(g => g.Key == recepcion.Id_Recepcion);
            if (dets is not null)
            {
                recepcion.Detalles = dets.ToList();
            }
        }

        return recepciones.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(Recepcion recepcion, CancellationToken cancellationToken = default)
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

            const string sqlSincronizarSecuencia = """
                INSERT INTO SecuenciaEntidades (nombre_entidad, ultimo_valor)
                VALUES ('Recepciones', (SELECT COALESCE(MAX(id_recepcion), 0) FROM Recepciones))
                ON CONFLICT(nombre_entidad) DO UPDATE SET
                    ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_recepcion), 0) FROM Recepciones));
                """;

            await conexion.ExecuteAsync(new CommandDefinition(sqlSincronizarSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlObtenerSiguienteId = """
                UPDATE SecuenciaEntidades 
                SET ultimo_valor = ultimo_valor + 1 
                WHERE nombre_entidad = 'Recepciones';

                SELECT ultimo_valor 
                FROM SecuenciaEntidades 
                WHERE nombre_entidad = 'Recepciones';
                """;

            var nuevoIdRecepcion = await conexion.ExecuteScalarAsync<int>(
                new CommandDefinition(sqlObtenerSiguienteId, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlInsertRecepcion = """
                INSERT INTO Recepciones (id_recepcion, fecha_recepcion, id_pedido, procesada_en_inventario)
                VALUES (@IdRecepcion, @FechaRecepcion, @IdPedido, @ProcesadaEnInventario);
                """;

            await conexion.ExecuteAsync(new CommandDefinition(sqlInsertRecepcion, new
            {
                IdRecepcion = nuevoIdRecepcion,
                FechaRecepcion = recepcion.FechaRecepcion.ToString("yyyy-MM-dd HH:mm:ss"),
                IdPedido = recepcion.PedidoId,
                ProcesadaEnInventario = recepcion.ProcesadaEnInventario ? 1 : 0
            }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlSincDetalle = """
                INSERT INTO SecuenciaEntidades (nombre_entidad, ultimo_valor)
                VALUES ('DetalleRecepcion', (SELECT COALESCE(MAX(id_detalle_recepcion), 0) FROM DetalleRecepcion))
                ON CONFLICT(nombre_entidad) DO UPDATE SET
                    ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_detalle_recepcion), 0) FROM DetalleRecepcion));
                """;
            await conexion.ExecuteAsync(new CommandDefinition(sqlSincDetalle, transaction: transaccion, cancellationToken: cancellationToken));

            foreach (var detalle in recepcion.Detalles)
            {
                const string sqlObtenerIdDetalle = """
                    UPDATE SecuenciaEntidades 
                    SET ultimo_valor = ultimo_valor + 1 
                    WHERE nombre_entidad = 'DetalleRecepcion';

                    SELECT ultimo_valor 
                    FROM SecuenciaEntidades 
                    WHERE nombre_entidad = 'DetalleRecepcion';
                    """;

                var nuevoIdDetalle = await conexion.ExecuteScalarAsync<int>(
                    new CommandDefinition(sqlObtenerIdDetalle, transaction: transaccion, cancellationToken: cancellationToken));

                const string sqlInsertDetalle = """
                    INSERT INTO DetalleRecepcion (id_detalle_recepcion, cantidad, precio_unitario, fecha_ingreso, id_recepcion, id_producto)
                    VALUES (@IdDetalleRecepcion, @Cantidad, @PrecioUnitario, @FechaIngreso, @IdRecepcion, @IdProducto);
                    """;

                await conexion.ExecuteAsync(new CommandDefinition(sqlInsertDetalle, new
                {
                    IdDetalleRecepcion = nuevoIdDetalle,
                    detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    FechaIngreso = detalle.FechaIngreso.ToString("yyyy-MM-dd HH:mm:ss"),
                    IdRecepcion = nuevoIdRecepcion,
                    IdProducto = detalle.IdProducto
                }, transaction: transaccion, cancellationToken: cancellationToken));
            }

            transaccion.Commit();
            return nuevoIdRecepcion;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task ActualizarAsync(Recepcion recepcion, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlUpdateRecepcion = """
                UPDATE Recepciones 
                SET fecha_recepcion = @FechaRecepcion,
                    id_pedido = @IdPedido,
                    procesada_en_inventario = @ProcesadaEnInventario
                WHERE id_recepcion = @Id;
                """;

            await conexion.ExecuteAsync(new CommandDefinition(sqlUpdateRecepcion, new
            {
                recepcion.Id,
                FechaRecepcion = recepcion.FechaRecepcion.ToString("yyyy-MM-dd HH:mm:ss"),
                IdPedido = recepcion.PedidoId,
                ProcesadaEnInventario = recepcion.ProcesadaEnInventario ? 1 : 0
            }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlDeleteDetalles = "DELETE FROM DetalleRecepcion WHERE id_recepcion = @Id;";
            await conexion.ExecuteAsync(new CommandDefinition(sqlDeleteDetalles, new { Id = recepcion.Id }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlSincDetalle = """
                INSERT INTO SecuenciaEntidades (nombre_entidad, ultimo_valor)
                VALUES ('DetalleRecepcion', (SELECT COALESCE(MAX(id_detalle_recepcion), 0) FROM DetalleRecepcion))
                ON CONFLICT(nombre_entidad) DO UPDATE SET
                    ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_detalle_recepcion), 0) FROM DetalleRecepcion));
                """;
            await conexion.ExecuteAsync(new CommandDefinition(sqlSincDetalle, transaction: transaccion, cancellationToken: cancellationToken));

            foreach (var detalle in recepcion.Detalles)
            {
                const string sqlObtenerIdDetalle = """
                    UPDATE SecuenciaEntidades 
                    SET ultimo_valor = ultimo_valor + 1 
                    WHERE nombre_entidad = 'DetalleRecepcion';

                    SELECT ultimo_valor 
                    FROM SecuenciaEntidades 
                    WHERE nombre_entidad = 'DetalleRecepcion';
                    """;

                var nuevoIdDetalle = await conexion.ExecuteScalarAsync<int>(
                    new CommandDefinition(sqlObtenerIdDetalle, transaction: transaccion, cancellationToken: cancellationToken));

                const string sqlInsertDetalle = """
                    INSERT INTO DetalleRecepcion (id_detalle_recepcion, cantidad, precio_unitario, fecha_ingreso, id_recepcion, id_producto)
                    VALUES (@IdDetalleRecepcion, @Cantidad, @PrecioUnitario, @FechaIngreso, @IdRecepcion, @IdProducto);
                    """;

                await conexion.ExecuteAsync(new CommandDefinition(sqlInsertDetalle, new
                {
                    IdDetalleRecepcion = nuevoIdDetalle,
                    detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    FechaIngreso = detalle.FechaIngreso.ToString("yyyy-MM-dd HH:mm:ss"),
                    IdRecepcion = recepcion.Id,
                    IdProducto = detalle.IdProducto
                }, transaction: transaccion, cancellationToken: cancellationToken));
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
            const string sqlDeleteDetalles = "DELETE FROM DetalleRecepcion WHERE id_recepcion = @Id;";
            await conexion.ExecuteAsync(new CommandDefinition(sqlDeleteDetalles, new { Id = id }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlDeleteRecepcion = "DELETE FROM Recepciones WHERE id_recepcion = @Id;";
            await conexion.ExecuteAsync(new CommandDefinition(sqlDeleteRecepcion, new { Id = id }, transaction: transaccion, cancellationToken: cancellationToken));

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }
}
