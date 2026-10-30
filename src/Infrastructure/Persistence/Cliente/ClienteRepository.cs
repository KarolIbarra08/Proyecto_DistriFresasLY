using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Cliente;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ClienteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                id_cliente AS Id_Cliente, 
                nombre AS Nombre, 
                cedula_nit AS Cedula_Nit, 
                telefono AS Telefono, 
                direccion AS Direccion, 
                tipo_de_negocio AS Tipo_De_Negocio, 
                activo AS Activo 
            FROM Clientes 
            WHERE id_cliente = @Id;
            """;
        
        var registro = await conexion.QueryFirstOrDefaultAsync<ClienteRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                id_cliente AS Id_Cliente, 
                nombre AS Nombre, 
                cedula_nit AS Cedula_Nit, 
                telefono AS Telefono, 
                direccion AS Direccion, 
                tipo_de_negocio AS Tipo_De_Negocio, 
                activo AS Activo 
            FROM Clientes;
            """;
        
        var registros = await conexion.QueryAsync<ClienteRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<bool> ExistePorDocumentoAsync(string cedulaNit, int? idExcluir = null, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT COUNT(1) 
            FROM Clientes 
            WHERE cedula_nit = @CedulaNit 
              AND (@IdExcluir IS NULL OR id_cliente <> @IdExcluir);
            """;
        
        var count = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { CedulaNit = cedulaNit, IdExcluir = idExcluir }, cancellationToken: cancellationToken));

        return count > 0;
    }

    public async Task<bool> TienePedidosAsociadosAsync(int idCliente, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();

        // 1. Verificar si la tabla Pedidos existe en SQLite (case-insensitive)
        const string sqlTablaExiste = """
            SELECT COUNT(1) 
            FROM sqlite_master 
            WHERE type = 'table' AND LOWER(name) = 'pedidos';
            """;

        var existeTabla = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sqlTablaExiste, cancellationToken: cancellationToken));

        if (existeTabla == 0)
        {
            return false;
        }

        // 2. Obtener la información real de columnas de la tabla Pedidos
        const string sqlColumnas = "PRAGMA table_info(Pedidos);";
        var columnas = await conexion.QueryAsync<dynamic>(
            new CommandDefinition(sqlColumnas, cancellationToken: cancellationToken));

        var nombresColumnas = columnas.Select(c => ((string)c.name).ToLower()).ToList();

        // 3. Identificar la columna de clave foránea que referencia al cliente
        string? columnaFk = null;
        if (nombresColumnas.Contains("id_cliente")) columnaFk = "id_cliente";
        else if (nombresColumnas.Contains("cliente_id")) columnaFk = "cliente_id";
        else if (nombresColumnas.Contains("idcliente")) columnaFk = "idcliente";
        else if (nombresColumnas.Contains("clienteid")) columnaFk = "clienteid";

        // Si la tabla Pedidos no tiene ninguna columna vinculada a cliente, no hay dependencias
        if (columnaFk is null)
        {
            return false;
        }

        // 4. Consultar usando el nombre exacto de la columna detectada
        string sqlQuery = $"SELECT COUNT(1) FROM Pedidos WHERE {columnaFk} = @Id;";

        var cantidad = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sqlQuery, new { Id = idCliente }, cancellationToken: cancellationToken));

        return cantidad > 0;
    }

    public async Task<int> AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open)
        {
            conexion.Open();
        }

        using var transaccion = conexion.BeginTransaction();

        try
        {
            // 1. Asegurar la existencia de la tabla de secuencias
            const string sqlInitSecuencia = """
                CREATE TABLE IF NOT EXISTS SecuenciaEntidades (
                    nombre_entidad TEXT PRIMARY KEY,
                    ultimo_valor INTEGER NOT NULL
                );
                """;

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlInitSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

            // 2. Sincronizar la secuencia con el ID máximo actual de la tabla Clientes
            const string sqlSincronizarSecuencia = """
                INSERT INTO SecuenciaEntidades (nombre_entidad, ultimo_valor)
                VALUES ('Cliente', (SELECT COALESCE(MAX(id_cliente), 0) FROM Clientes))
                ON CONFLICT(nombre_entidad) DO UPDATE SET
                    ultimo_valor = MAX(SecuenciaEntidades.ultimo_valor, (SELECT COALESCE(MAX(id_cliente), 0) FROM Clientes));
                """;

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlSincronizarSecuencia, transaction: transaccion, cancellationToken: cancellationToken));

            // 3. Incrementar y obtener el consecutivo libre
            const string sqlObtenerSiguienteId = """
                UPDATE SecuenciaEntidades 
                SET ultimo_valor = ultimo_valor + 1 
                WHERE nombre_entidad = 'Cliente';

                SELECT ultimo_valor 
                FROM SecuenciaEntidades 
                WHERE nombre_entidad = 'Cliente';
                """;

            var nuevoIdCliente = await conexion.ExecuteScalarAsync<int>(
                new CommandDefinition(sqlObtenerSiguienteId, transaction: transaccion, cancellationToken: cancellationToken));

            // 4. Insertar el cliente con su ID asignado
            const string sqlInsert = """
                INSERT INTO Clientes (id_cliente, nombre, cedula_nit, telefono, direccion, tipo_de_negocio, activo)
                VALUES (@IdCliente, @Nombre, @CedulaNit, @Telefono, @Direccion, @TipoNegocio, @Activo);
                """;

            var parametros = new
            {
                IdCliente = nuevoIdCliente,
                cliente.Nombre,
                CedulaNit = cliente.Documento.Value,
                Telefono = cliente.Telefono.Valor,
                cliente.Direccion,
                cliente.TipoNegocio,
                Activo = cliente.Activo ? 1 : 0
            };

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlInsert, parametros, transaction: transaccion, cancellationToken: cancellationToken));

            transaccion.Commit();
            return nuevoIdCliente;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task ActualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            UPDATE Clientes 
            SET nombre = @Nombre, 
                cedula_nit = @CedulaNit, 
                telefono = @Telefono, 
                direccion = @Direccion, 
                tipo_de_negocio = @TipoNegocio 
            WHERE id_cliente = @Id;
            """;

        var parametros = new
        {
            cliente.Id,
            cliente.Nombre,
            CedulaNit = cliente.Documento.Value,
            Telefono = cliente.Telefono.Valor,
            cliente.Direccion,
            cliente.TipoNegocio
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = "DELETE FROM Clientes WHERE id_cliente = @Id;";
        
        await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}