using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Proveedores;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProveedorRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_proveedor AS Id_Proveedor,
                nombre AS Nombre,
                apellido AS Apellido,
                cedula_nit AS Cedula_Nit,
                telefono AS Telefono,
                direccion AS Direccion,
                nombre_empresa AS Nombre_Empresa
            FROM Proveedores
            WHERE id_proveedor = @Id;
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<ProveedorRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<Proveedor?> ObtenerPorCedulaNitAsync(string cedulaNit, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_proveedor AS Id_Proveedor,
                nombre AS Nombre,
                apellido AS Apellido,
                cedula_nit AS Cedula_Nit,
                telefono AS Telefono,
                direccion AS Direccion,
                nombre_empresa AS Nombre_Empresa
            FROM Proveedores
            WHERE LOWER(cedula_nit) = LOWER(@CedulaNit);
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<ProveedorRegistro>(
            new CommandDefinition(sql, new { CedulaNit = cedulaNit.Trim() }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<IReadOnlyList<Proveedor>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_proveedor AS Id_Proveedor,
                nombre AS Nombre,
                apellido AS Apellido,
                cedula_nit AS Cedula_Nit,
                telefono AS Telefono,
                direccion AS Direccion,
                nombre_empresa AS Nombre_Empresa
            FROM Proveedores;
            """;

        var registros = await conexion.QueryAsync<ProveedorRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            INSERT INTO Proveedores (nombre, apellido, cedula_nit, telefono, direccion, nombre_empresa)
            VALUES (@Nombre, @Apellido, @CedulaNit, @Telefono, @Direccion, @NombreEmpresa);

            SELECT last_insert_rowid();
            """;

        var parametros = new
        {
            Nombre = proveedor.Nombre,
            Apellido = proveedor.Apellido,
            CedulaNit = proveedor.Documento.Value,
            Telefono = proveedor.Telefono.Valor,
            Direccion = proveedor.Direccion,
            NombreEmpresa = proveedor.NombreEmpresa
        };

        return await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task ActualizarAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            UPDATE Proveedores
            SET nombre = @Nombre,
                apellido = @Apellido,
                cedula_nit = @CedulaNit,
                telefono = @Telefono,
                direccion = @Direccion,
                nombre_empresa = @NombreEmpresa
            WHERE id_proveedor = @Id;
            """;

        var parametros = new
        {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre,
            Apellido = proveedor.Apellido,
            CedulaNit = proveedor.Documento.Value,
            Telefono = proveedor.Telefono.Valor,
            Direccion = proveedor.Direccion,
            NombreEmpresa = proveedor.NombreEmpresa
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = "DELETE FROM Proveedores WHERE id_proveedor = @Id;";

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}