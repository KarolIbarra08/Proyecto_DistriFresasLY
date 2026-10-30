using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Productos;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();

        const string sqlInsumo = """
            SELECT 
                p.id_producto AS Id_Producto,
                'insumo' AS Tipo_Producto,
                pi.nombre AS Nombre,
                pi.tipo_insumo AS Tipo_Insumo,
                pi.unidad_medida AS Unidad_Medida,
                pi.descripcion AS Descripcion
            FROM Producto p
            INNER JOIN ProductoInsumo pi ON p.id_producto = pi.id_producto
            WHERE p.id_producto = @Id;
            """;

        var registroInsumo = await conexion.QueryFirstOrDefaultAsync<ProductoRegistro>(
            new CommandDefinition(sqlInsumo, new { Id = id }, cancellationToken: cancellationToken));

        if (registroInsumo is not null)
        {
            return registroInsumo.ToEntidad();
        }

        const string sqlFresa = """
            SELECT 
                p.id_producto AS Id_Producto,
                'fresa' AS Tipo_Producto,
                pf.calibre AS Calibre,
                pf.calidad AS Calidad,
                pf.peso AS Peso,
                pf.descripcion AS Descripcion
            FROM Producto p
            INNER JOIN ProductoFresa pf ON p.id_producto = pf.id_producto
            WHERE p.id_producto = @Id;
            """;

        var registroFresa = await conexion.QueryFirstOrDefaultAsync<ProductoRegistro>(
            new CommandDefinition(sqlFresa, new { Id = id }, cancellationToken: cancellationToken));

        return registroFresa?.ToEntidad();
    }

    public async Task<IReadOnlyList<Producto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();

        const string sqlInsumos = """
            SELECT 
                p.id_producto AS Id_Producto,
                'insumo' AS Tipo_Producto,
                pi.nombre AS Nombre,
                pi.tipo_insumo AS Tipo_Insumo,
                pi.unidad_medida AS Unidad_Medida,
                pi.descripcion AS Descripcion
            FROM Producto p
            INNER JOIN ProductoInsumo pi ON p.id_producto = pi.id_producto;
            """;

        const string sqlFresas = """
            SELECT 
                p.id_producto AS Id_Producto,
                'fresa' AS Tipo_Producto,
                pf.calibre AS Calibre,
                pf.calidad AS Calidad,
                pf.peso AS Peso,
                pf.descripcion AS Descripcion
            FROM Producto p
            INNER JOIN ProductoFresa pf ON p.id_producto = pf.id_producto;
            """;

        var insumos = await conexion.QueryAsync<ProductoRegistro>(
            new CommandDefinition(sqlInsumos, cancellationToken: cancellationToken));

        var fresas = await conexion.QueryAsync<ProductoRegistro>(
            new CommandDefinition(sqlFresas, cancellationToken: cancellationToken));

        return insumos.Concat(fresas)
                      .Select(r => r.ToEntidad())
                      .ToList();
    }

    // ==========================================
    // SECCIÓN INSUMOS
    // ==========================================

    public async Task<IReadOnlyList<ProductoInsumo>> ObtenerTodosInsumosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                p.id_producto AS Id_Producto,
                'insumo' AS Tipo_Producto,
                pi.nombre AS Nombre,
                pi.tipo_insumo AS Tipo_Insumo,
                pi.unidad_medida AS Unidad_Medida,
                pi.descripcion AS Descripcion
            FROM Producto p
            INNER JOIN ProductoInsumo pi ON p.id_producto = pi.id_producto
            WHERE LOWER(p.tipo_producto) = 'insumo';
            """;

        var registros = await conexion.QueryAsync<ProductoRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad())
                        .OfType<ProductoInsumo>()
                        .ToList();
    }

    public async Task<int> AgregarInsumoAsync(ProductoInsumo insumo, CancellationToken cancellationToken = default)
{
    using var conexion = _connectionFactory.CrearConexion();
    if (conexion.State != ConnectionState.Open)
    {
        conexion.Open();
    }

    using var transaccion = conexion.BeginTransaction();

    try
    {
        // 1. Crear la tabla de secuencias si no existe
        const string sqlInitSecuencia = """
            CREATE TABLE IF NOT EXISTS SecuenciaEntidades (
                nombre_entidad TEXT PRIMARY KEY,
                ultimo_valor INTEGER NOT NULL
            );
            """;

        await conexion.ExecuteAsync(
            new CommandDefinition(sqlInitSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

        // 2. Sincronizar secuencia: asegura iniciar mínimo en 100 y detecta si ya existen IDs de Insumo mayores
        const string sqlSincronizarSecuencia = """
            INSERT INTO SecuenciaEntidades (nombre_entidad, ultimo_valor)
            VALUES ('Insumo', MAX(100, (SELECT COALESCE(MAX(id_producto), 100) FROM Producto WHERE LOWER(tipo_producto) = 'insumo')))
            ON CONFLICT(nombre_entidad) DO UPDATE SET
                ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, MAX(100, (SELECT COALESCE(MAX(id_producto), 100) FROM Producto WHERE LOWER(tipo_producto) = 'insumo')));
            """;

        await conexion.ExecuteAsync(
            new CommandDefinition(sqlSincronizarSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

        // 3. Incrementar y obtener el siguiente ID libre exclusivo para Insumos (101, 102, ...)
        const string sqlObtenerSiguienteId = """
            UPDATE SecuenciaEntidades 
            SET ultimo_valor = ultimo_valor + 1 
            WHERE nombre_entidad = 'Insumo';

            SELECT ultimo_valor 
            FROM SecuenciaEntidades 
            WHERE nombre_entidad = 'Insumo';
            """;

        var nuevoIdInsumo = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sqlObtenerSiguienteId, transaction: transaccion, cancellationToken: cancellationToken));

        // 4. Insertar en Producto con el ID explícito
        const string sqlProductoPadre = """
            INSERT INTO Producto (id_producto, tipo_producto)
            VALUES (@IdProducto, 'Insumo');
            """;

        await conexion.ExecuteAsync(
            new CommandDefinition(sqlProductoPadre, new { IdProducto = nuevoIdInsumo }, transaction: transaccion, cancellationToken: cancellationToken));

        // 5. Insertar en ProductoInsumo
        const string sqlProductoInsumo = """
            INSERT INTO ProductoInsumo (id_producto, nombre, tipo_insumo, unidad_medida, descripcion)
            VALUES (@IdProducto, @Nombre, @TipoInsumo, @UnidadMedida, @Descripcion);
            """;

        var parametrosInsumo = new
        {
            IdProducto = nuevoIdInsumo,
            insumo.Nombre,
            insumo.TipoInsumo,
            insumo.UnidadMedida,
            insumo.Descripcion
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sqlProductoInsumo, parametrosInsumo, transaction: transaccion, cancellationToken: cancellationToken));

        transaccion.Commit();
        return nuevoIdInsumo;
    }
    catch
    {
        transaccion.Rollback();
        throw;
    }
}

    public async Task ActualizarInsumoAsync(ProductoInsumo insumo, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            UPDATE ProductoInsumo 
            SET nombre = @Nombre, 
                tipo_insumo = @TipoInsumo, 
                unidad_medida = @UnidadMedida, 
                descripcion = @Descripcion 
            WHERE id_producto = @Id;
            """;

        var parametros = new
        {
            insumo.Id,
            insumo.Nombre,
            insumo.TipoInsumo,
            insumo.UnidadMedida,
            insumo.Descripcion
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    // ==========================================
    // SECCIÓN FRESAS
    // ==========================================

    public async Task<IReadOnlyList<ProductoFresa>> ObtenerTodasFresasAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                p.id_producto AS Id_Producto,
                'fresa' AS Tipo_Producto,
                pf.calibre AS Calibre,
                pf.calidad AS Calidad,
                pf.peso AS Peso,
                pf.descripcion AS Descripcion
            FROM Producto p
            INNER JOIN ProductoFresa pf ON p.id_producto = pf.id_producto
            WHERE LOWER(p.tipo_producto) = 'fresa';
            """;

        var registros = await conexion.QueryAsync<ProductoRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad())
                        .OfType<ProductoFresa>()
                        .ToList();
    }

    public async Task<bool> ExisteCalibreAsync(int calibre, int? idExcluir = null, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT COUNT(1) 
            FROM ProductoFresa 
            WHERE calibre = @Calibre AND (@IdExcluir IS NULL OR id_producto != @IdExcluir);
            """;

        var cantidad = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { Calibre = calibre, IdExcluir = idExcluir }, cancellationToken: cancellationToken));

        return cantidad > 0;
    }

   public async Task<int> AgregarFresaAsync(ProductoFresa fresa, CancellationToken cancellationToken = default)
{
    using var conexion = _connectionFactory.CrearConexion();
    if (conexion.State != ConnectionState.Open)
    {
        conexion.Open();
    }

    using var transaccion = conexion.BeginTransaction();

    try
    {
        // 1. Asegurar que exista la tabla de secuencias
        const string sqlInitSecuencia = """
            CREATE TABLE IF NOT EXISTS SecuenciaEntidades (
                nombre_entidad TEXT PRIMARY KEY,
                ultimo_valor INTEGER NOT NULL
            );
            """;

        await conexion.ExecuteAsync(
            new CommandDefinition(sqlInitSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

        // 2. Sincronizar la secuencia de Fresas con el máximo ID actual registrado en SQLite
        const string sqlSincronizarSecuencia = """
            INSERT INTO SecuenciaEntidades (nombre_entidad, ultimo_valor)
            VALUES ('Fresa', (SELECT COALESCE(MAX(id_producto), 0) FROM Producto WHERE LOWER(tipo_producto) = 'fresa'))
            ON CONFLICT(nombre_entidad) DO UPDATE SET
                ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_producto), 0) FROM Producto WHERE LOWER(tipo_producto) = 'fresa'));
            """;

        await conexion.ExecuteAsync(
            new CommandDefinition(sqlSincronizarSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

        // 3. Incrementar y obtener el siguiente ID consecutivo disponible para Fresas (1, 2, 3...)
        const string sqlObtenerSiguienteId = """
            UPDATE SecuenciaEntidades 
            SET ultimo_valor = ultimo_valor + 1 
            WHERE nombre_entidad = 'Fresa';

            SELECT ultimo_valor 
            FROM SecuenciaEntidades 
            WHERE nombre_entidad = 'Fresa';
            """;

        var nuevoIdFresa = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sqlObtenerSiguienteId, transaction: transaccion, cancellationToken: cancellationToken));

        // 4. Insertar en la tabla padre Producto con el ID consecutivo asignado
        const string sqlProductoPadre = """
            INSERT INTO Producto (id_producto, tipo_producto)
            VALUES (@IdProducto, 'Fresa');
            """;

        await conexion.ExecuteAsync(
            new CommandDefinition(sqlProductoPadre, new { IdProducto = nuevoIdFresa }, transaction: transaccion, cancellationToken: cancellationToken));

        // 5. Insertar los datos correspondientes en ProductoFresa
        const string sqlProductoFresa = """
            INSERT INTO ProductoFresa (id_producto, calibre, calidad, peso, descripcion)
            VALUES (@IdProducto, @Calibre, @Calidad, @Peso, @Descripcion);
            """;

        var parametros = new
        {
            IdProducto = nuevoIdFresa,
            Calibre = fresa.Calibre.Value,
            fresa.Calidad,
            Peso = fresa.Peso.Value,
            fresa.Descripcion
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sqlProductoFresa, parametros, transaction: transaccion, cancellationToken: cancellationToken));

        transaccion.Commit();
        return nuevoIdFresa;
    }
    catch
    {
        transaccion.Rollback();
        throw;
    }
}

    public async Task ActualizarFresaAsync(ProductoFresa fresa, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            UPDATE ProductoFresa 
            SET calibre = @Calibre, 
                calidad = @Calidad, 
                peso = @Peso, 
                descripcion = @Descripcion 
            WHERE id_producto = @Id;
            """;

        var parametros = new
        {
            fresa.Id,
            Calibre = fresa.Calibre.Value,
            fresa.Calidad,
            Peso = fresa.Peso.Value,
            fresa.Descripcion
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    // ==========================================
    // ELIMINACIÓN GENERAL
    // ==========================================

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open)
        {
            conexion.Open();
        }

        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlEliminarInsumo = "DELETE FROM ProductoInsumo WHERE id_producto = @Id;";
            await conexion.ExecuteAsync(
                new CommandDefinition(sqlEliminarInsumo, new { Id = id }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlEliminarFresa = "DELETE FROM ProductoFresa WHERE id_producto = @Id;";
            await conexion.ExecuteAsync(
                new CommandDefinition(sqlEliminarFresa, new { Id = id }, transaction: transaccion, cancellationToken: cancellationToken));

            const string sqlEliminarPadre = "DELETE FROM Producto WHERE id_producto = @Id;";
            await conexion.ExecuteAsync(
                new CommandDefinition(sqlEliminarPadre, new { Id = id }, transaction: transaccion, cancellationToken: cancellationToken));

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }
}