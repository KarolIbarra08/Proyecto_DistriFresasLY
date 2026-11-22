using Dapper;
using DistriFresasLY.Application.Abstractions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Cliente;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

internal sealed class ClienteRepository : RepositoryBase, IClienteRepository
{
    private const string SelectColumnas = """
        SELECT id_cliente AS Id,
               nombre AS Nombre,
               cedula_nit AS CedulaNit,
               telefono AS Telefono,
               direccion AS Direccion,
               tipo_de_negocio AS TipoNegocio,
               activo AS Activo
        FROM Clientes
        """;

    public ClienteRepository(
        IDbConnectionFactory connectionFactory,
        TimeProvider reloj,
        IUsuarioActual usuarioActual)
        : base(connectionFactory, reloj, usuarioActual)
    {
    }

    public Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        => BuscarAsync(null, null, cancellationToken);

    public async Task<IReadOnlyList<Cliente>> BuscarAsync(
        string? tipoNegocio = null,
        bool? activo = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            {SelectColumnas}
            WHERE (@TipoNegocio IS NULL OR tipo_de_negocio = @TipoNegocio)
              AND (@Activo IS NULL OR activo = @Activo)
            ORDER BY nombre;
            """;

        using var conexion = ConnectionFactory.CrearConexion();

        var registros = await conexion.QueryAsync<ClienteRegistro>(
            new CommandDefinition(
                sql,
                new { TipoNegocio = tipoNegocio, Activo = activo },
                cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"{SelectColumnas} WHERE id_cliente = @Id;";

        using var conexion = ConnectionFactory.CrearConexion();

        var registro = await conexion.QuerySingleOrDefaultAsync<ClienteRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<bool> ExistePorDocumentoAsync(
        string cedulaNit,
        int? idExcluir = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM Clientes
                WHERE cedula_nit = @CedulaNit
                  AND (@IdExcluir IS NULL OR id_cliente <> @IdExcluir)
            );
            """;

        using var conexion = ConnectionFactory.CrearConexion();

        return await conexion.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                sql,
                new { CedulaNit = cedulaNit.Trim(), IdExcluir = idExcluir },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> TienePedidosAsociadosAsync(
        int idCliente,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM Venta WHERE id_cliente = @IdCliente
                UNION ALL
                SELECT 1 FROM PagoCliente WHERE id_cliente = @IdCliente
            );
            """;

        using var conexion = ConnectionFactory.CrearConexion();

        return await conexion.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { IdCliente = idCliente }, cancellationToken: cancellationToken));
    }

    public async Task<int> AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Clientes (nombre, cedula_nit, telefono, direccion, tipo_de_negocio, activo)
            VALUES (@Nombre, @CedulaNit, @Telefono, @Direccion, @TipoNegocio, @Activo)
            RETURNING id_cliente;
            """;

        var parametros = new
        {
            cliente.Nombre,
            CedulaNit = cliente.Documento.Value,
            Telefono = cliente.Telefono.Valor,
            cliente.Direccion,
            cliente.TipoNegocio,
            Activo = cliente.Activo ? 1 : 0
        };

        using var conexion = ConnectionFactory.CrearConexion();

        return await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Clientes
            SET nombre = @Nombre,
                cedula_nit = @CedulaNit,
                telefono = @Telefono,
                direccion = @Direccion,
                tipo_de_negocio = @TipoNegocio,
                activo = @Activo
            WHERE id_cliente = @Id;
            """;

        var parametros = new
        {
            cliente.Id,
            cliente.Nombre,
            CedulaNit = cliente.Documento.Value,
            Telefono = cliente.Telefono.Valor,
            cliente.Direccion,
            cliente.TipoNegocio,
            Activo = cliente.Activo ? 1 : 0
        };

        using var conexion = ConnectionFactory.CrearConexion();

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM Clientes WHERE id_cliente = @Id;";

        using var conexion = ConnectionFactory.CrearConexion();

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}