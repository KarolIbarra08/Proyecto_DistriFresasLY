using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Roles;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class RolRepository : IRolRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RolRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Rol?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                r.id_rol AS Id_Rol,
                r.nombre AS Nombre,
                r.descripcion AS Descripcion,
                p.nombre AS NombrePermiso
            FROM Roles r
            LEFT JOIN RolPermisos rp ON r.id_rol = rp.id_rol
            LEFT JOIN Permisos p ON rp.id_permiso = p.id_permiso
            WHERE r.id_rol = @Id;
            """;

        RolRegistro? rolRegistro = null;
        var permisos = new List<string>();

        await conexion.QueryAsync<RolRegistro, string?, RolRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken),
            (rol, nombrePermiso) =>
            {
                rolRegistro ??= rol;
                if (!string.IsNullOrWhiteSpace(nombrePermiso))
                {
                    permisos.Add(nombrePermiso);
                }
                return rol;
            },
            splitOn: "NombrePermiso"
        );

        if (rolRegistro is null) return null;

        return rolRegistro.ToEntidad(permisos);
    }

    public async Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                r.id_rol AS Id_Rol,
                r.nombre AS Nombre,
                r.descripcion AS Descripcion,
                p.nombre AS NombrePermiso
            FROM Roles r
            LEFT JOIN RolPermisos rp ON r.id_rol = rp.id_rol
            LEFT JOIN Permisos p ON rp.id_permiso = p.id_permiso
            WHERE LOWER(r.nombre) = LOWER(@Nombre);
            """;

        RolRegistro? rolRegistro = null;
        var permisos = new List<string>();

        await conexion.QueryAsync<RolRegistro, string?, RolRegistro>(
            new CommandDefinition(sql, new { Nombre = nombre.Trim() }, cancellationToken: cancellationToken),
            (rol, nombrePermiso) =>
            {
                rolRegistro ??= rol;
                if (!string.IsNullOrWhiteSpace(nombrePermiso))
                {
                    permisos.Add(nombrePermiso);
                }
                return rol;
            },
            splitOn: "NombrePermiso"
        );

        if (rolRegistro is null) return null;

        return rolRegistro.ToEntidad(permisos);
    }

    public async Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        const string sql = """
            SELECT 
                r.id_rol AS Id_Rol,
                r.nombre AS Nombre,
                r.descripcion AS Descripcion,
                p.nombre AS NombrePermiso
            FROM Roles r
            LEFT JOIN RolPermisos rp ON r.id_rol = rp.id_rol
            LEFT JOIN Permisos p ON rp.id_permiso = p.id_permiso;
            """;

        var rolDictionary = new Dictionary<int, (RolRegistro Rol, List<string> Permisos)>();

        await conexion.QueryAsync<RolRegistro, string?, RolRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken),
            (rol, nombrePermiso) =>
            {
                if (!rolDictionary.TryGetValue(rol.Id_Rol, out var entry))
                {
                    entry = (rol, new List<string>());
                    rolDictionary.Add(rol.Id_Rol, entry);
                }

                if (!string.IsNullOrWhiteSpace(nombrePermiso))
                {
                    entry.Permisos.Add(nombrePermiso);
                }

                return rol;
            },
            splitOn: "NombrePermiso"
        );

        return rolDictionary.Values
            .Select(e => e.Rol.ToEntidad(e.Permisos))
            .ToList();
    }

    public async Task<int> AgregarAsync(Rol rol, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlRol = """
                INSERT INTO Roles (nombre, descripcion)
                VALUES (@Nombre, @Descripcion);

                SELECT last_insert_rowid();
                """;

            var parametrosRol = new
            {
                Nombre = rol.Nombre.Trim(),
                Descripcion = rol.Descripcion.Trim()
            };

            var idRol = await conexion.ExecuteScalarAsync<int>(
                new CommandDefinition(sqlRol, parametrosRol, transaction: transaccion, cancellationToken: cancellationToken));

            if (rol.Permisos.Count > 0)
            {
                const string sqlInsertPermisos = """
                    INSERT OR IGNORE INTO Permisos (nombre) 
                    VALUES (LOWER(@NombrePermiso));

                    INSERT OR IGNORE INTO RolPermisos (id_rol, id_permiso)
                    SELECT @IdRol, id_permiso 
                    FROM Permisos 
                    WHERE LOWER(nombre) = LOWER(@NombrePermiso);
                    """;

                foreach (var nombrePermiso in rol.Permisos)
                {
                    await conexion.ExecuteAsync(
                        new CommandDefinition(sqlInsertPermisos, new { IdRol = idRol, NombrePermiso = nombrePermiso }, transaction: transaccion, cancellationToken: cancellationToken));
                }
            }

            transaccion.Commit();
            return idRol;
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }
    }

    public async Task ActualizarAsync(Rol rol, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();
        using var transaccion = conexion.BeginTransaction();

        try
        {
            const string sqlRol = """
                UPDATE Roles
                SET nombre = @Nombre,
                    descripcion = @Descripcion
                WHERE id_rol = @Id;
                """;

            var parametrosRol = new
            {
                Id = rol.Id,
                Nombre = rol.Nombre.Trim(),
                Descripcion = rol.Descripcion.Trim()
            };

            await conexion.ExecuteAsync(
                new CommandDefinition(sqlRol, parametrosRol, transaction: transaccion, cancellationToken: cancellationToken));


            const string sqlDeletePermisos = "DELETE FROM RolPermisos WHERE id_rol = @IdRol;";
            await conexion.ExecuteAsync(
                new CommandDefinition(sqlDeletePermisos, new { IdRol = rol.Id }, transaction: transaccion, cancellationToken: cancellationToken));

    
            if (rol.Permisos.Count > 0)
            {
                const string sqlInsertPermisos = """
                    INSERT OR IGNORE INTO Permisos (nombre) 
                    VALUES (LOWER(@NombrePermiso));

                    INSERT OR IGNORE INTO RolPermisos (id_rol, id_permiso)
                    SELECT @IdRol, id_permiso 
                    FROM Permisos 
                    WHERE LOWER(nombre) = LOWER(@NombrePermiso);
                    """;

                foreach (var nombrePermiso in rol.Permisos)
                {
                    await conexion.ExecuteAsync(
                        new CommandDefinition(sqlInsertPermisos, new { IdRol = rol.Id, NombrePermiso = nombrePermiso }, transaction: transaccion, cancellationToken: cancellationToken));
                }
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
        const string sql = "DELETE FROM Roles WHERE id_rol = @Id;";

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}