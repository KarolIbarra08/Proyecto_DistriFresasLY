using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Productos;
using DistriFresasLY.Infrastructure.Persistence.Registros;
using Microsoft.Data.Sqlite;

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
        // 1. Insertar en la tabla padre y obtener el ID consecutivo que asigna SQLite
        const string sqlProductoPadre = """
            INSERT INTO Producto (tipo_producto)
            VALUES ('Insumo')
            RETURNING id_producto;
            """;

        var nuevoIdInsumo = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sqlProductoPadre, transaction: transaccion, cancellationToken: cancellationToken));

        // 2. Insertar en ProductoInsumo usando ese mismo ID
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
        // 1. Insertar en la tabla padre y obtener el ID consecutivo que asigna SQLite
        const string sqlProductoPadre = """
            INSERT INTO Producto (tipo_producto)
            VALUES ('Fresa')
            RETURNING id_producto;
            """;

        var nuevoIdFresa = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sqlProductoPadre, transaction: transaccion, cancellationToken: cancellationToken));

        // 2. Insertar los datos correspondientes en ProductoFresa
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
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19 && ex.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
        {
            transaccion.Rollback();

            var referencias = await ObtenerReferenciasAsync(conexion, id, cancellationToken);
            var detalle = referencias.TieneReferencias
                ? referencias.Descripcion()
                : "registros relacionados en el sistema";

            var tipo = referencias.TipoProducto?.ToLowerInvariant() switch
            {
                "fresa" => "la fresa",
                "insumo" => "el insumo",
                _ => "el producto"
            };

            throw new ConflictoException(
                "Producto.ConReferencias",
                $"No se puede eliminar {tipo} con ID {id} porque tiene registros asociados: {detalle}. Elimine o reasigne esos registros primero.");
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    private static async Task<ReferenciasProducto> ObtenerReferenciasAsync(
        IDbConnection conexion,
        int id,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                (SELECT tipo_producto FROM Producto WHERE id_producto = @Id) AS TipoProducto,
                (SELECT COUNT(*) FROM ProductoDanado WHERE id_producto = @Id) AS ProductosDanados,
                (SELECT COUNT(*) FROM DetalleVenta WHERE id_producto = @Id) AS DetallesVenta,
                (SELECT COUNT(*) FROM DetallePedido WHERE id_producto = @Id) AS DetallesPedido,
                (SELECT COUNT(*) FROM DetalleRecepcion WHERE id_producto = @Id) AS DetallesRecepcion;
            """;

        return await conexion.QuerySingleAsync<ReferenciasProducto>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}

internal sealed class ReferenciasProducto
{
    public string? TipoProducto { get; set; }
    public int ProductosDanados { get; set; }
    public int DetallesVenta { get; set; }
    public int DetallesPedido { get; set; }
    public int DetallesRecepcion { get; set; }

    public bool TieneReferencias =>
        ProductosDanados > 0 || DetallesVenta > 0 || DetallesPedido > 0 || DetallesRecepcion > 0;

    public string Descripcion()
    {
        var partes = new List<string>();

        if (ProductosDanados > 0)
        {
            partes.Add(ProductosDanados == 1
                ? "1 registro de producto dañado"
                : $"{ProductosDanados} registros de productos dañados");
        }

        if (DetallesVenta > 0)
        {
            partes.Add(DetallesVenta == 1
                ? "1 detalle de venta"
                : $"{DetallesVenta} detalles de venta");
        }

        if (DetallesPedido > 0)
        {
            partes.Add(DetallesPedido == 1
                ? "1 detalle de pedido"
                : $"{DetallesPedido} detalles de pedido");
        }

        if (DetallesRecepcion > 0)
        {
            partes.Add(DetallesRecepcion == 1
                ? "1 detalle de recepción"
                : $"{DetallesRecepcion} detalles de recepción");
        }

        return string.Join(", ", partes);
    }
}