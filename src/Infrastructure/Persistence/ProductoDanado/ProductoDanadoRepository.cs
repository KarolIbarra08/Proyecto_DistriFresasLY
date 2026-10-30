using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.ProductosDanados;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class ProductoDanadoRepository : IProductoDanadoRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductoDanadoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ProductoDanado?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                id_producto_danado AS Id_Producto_Danado,
                id_producto AS Id_Producto,
                tipo_producto AS Tipo_Producto,
                cantidad AS Cantidad,
                identificacion AS Identificacion,
                valor AS Valor,
                fecha_producto_danado AS Fecha_Producto_Danado,
                motivo AS Motivo
            FROM ProductoDanado
            WHERE id_producto_danado = @Id;
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<ProductoDanadoRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<IReadOnlyList<ProductoDanado>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                id_producto_danado AS Id_Producto_Danado,
                id_producto AS Id_Producto,
                tipo_producto AS Tipo_Producto,
                cantidad AS Cantidad,
                identificacion AS Identificacion,
                valor AS Valor,
                fecha_producto_danado AS Fecha_Producto_Danado,
                motivo AS Motivo
            FROM ProductoDanado;
            """;

        var registros = await conexion.QueryAsync<ProductoDanadoRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(ProductoDanado productoDanado, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            INSERT INTO ProductoDanado (id_producto, tipo_producto, cantidad, identificacion, valor, fecha_producto_danado, motivo)
            VALUES (@IdProducto, @TipoProducto, @Cantidad, @Identificacion, @Valor, @FechaProductoDanado, @Motivo);
            
            SELECT last_insert_rowid();
            """;

        var parametros = new
        {
            IdProducto = productoDanado.ProductoId,
            TipoProducto = productoDanado.TipoProducto,
            Cantidad = productoDanado.Cantidad.Value, 
            Identificacion = productoDanado.Identificacion,
            Valor = (double)productoDanado.Valor,
           FechaProductoDanado = productoDanado.FechaProductoDanado,
            Motivo = productoDanado.Motivo
        };

        return await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task ActualizarAsync(ProductoDanado productoDanado, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            UPDATE ProductoDanado
            SET id_producto = @IdProducto,
                tipo_producto = @TipoProducto,
                cantidad = @Cantidad,
                identificacion = @Identificacion,
                valor = @Valor,
                fecha_producto_danado = @FechaProductoDanado,
                motivo = @Motivo
            WHERE id_producto_danado = @Id;
            """;

        var parametros = new
        {
            Id = productoDanado.Id,
            IdProducto = productoDanado.ProductoId,
            TipoProducto = productoDanado.TipoProducto,
            Cantidad = productoDanado.Cantidad.Value, 
            Identificacion = productoDanado.Identificacion,
            Valor = (double)productoDanado.Valor,
            FechaProductoDanado = productoDanado.FechaProductoDanado,
            Motivo = productoDanado.Motivo
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }
}