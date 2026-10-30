using System.Data;
using Dapper;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Entities.Usuarios;
using DistriFresasLY.Infrastructure.Persistence.Registros;

namespace DistriFresasLY.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_usuario AS Id_Usuario,
                nombre AS Nombre,
                apellido AS Apellido,
                usuario AS Nombre_Usuario,
                contrasena AS Contrasena,
                id_rol AS Rol,
                estado AS Estado
            FROM Usuarios
            WHERE id_usuario = @Id;
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<UsuarioRegistro>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_usuario AS Id_Usuario,
                nombre AS Nombre,
                apellido AS Apellido,
                usuario AS Nombre_Usuario,
                contrasena AS Contrasena,
                id_rol AS Rol,
                estado AS Estado
            FROM Usuarios
            WHERE LOWER(usuario) = LOWER(@NombreUsuario);
            """;

        var registro = await conexion.QueryFirstOrDefaultAsync<UsuarioRegistro>(
            new CommandDefinition(sql, new { NombreUsuario = nombreUsuario.Trim() }, cancellationToken: cancellationToken));

        return registro?.ToEntidad();
    }

    public async Task<IReadOnlyList<Usuario>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            SELECT 
                id_usuario AS Id_Usuario,
                nombre AS Nombre,
                apellido AS Apellido,
                usuario AS Nombre_Usuario,
                contrasena AS Contrasena,
                id_rol AS Rol,
                estado AS Estado
            FROM Usuarios;
            """;

        var registros = await conexion.QueryAsync<UsuarioRegistro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return registros.Select(r => r.ToEntidad()).ToList();
    }

    public async Task<int> AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            INSERT INTO Usuarios (nombre, apellido, usuario, contrasena, id_rol, estado)
            VALUES (@Nombre, @Apellido, @NombreUsuario, @Contrasena, @RolId, @Estado);

            SELECT last_insert_rowid();
            """;

       
        _ = int.TryParse(usuario.Rol, out int idRol);

        var parametros = new
        {
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            NombreUsuario = usuario.NombreUsuario,
            Contrasena = usuario.Contrasena,
            RolId = idRol,
            Estado = usuario.Estado ? 1 : 0
        };

        return await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task ActualizarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = """
            UPDATE Usuarios
            SET nombre = @Nombre,
                apellido = @Apellido,
                usuario = @NombreUsuario,
                contrasena = @Contrasena,
                id_rol = @RolId,
                estado = @Estado
            WHERE id_usuario = @Id;
            """;

        _ = int.TryParse(usuario.Rol, out int idRol);

        var parametros = new
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            NombreUsuario = usuario.NombreUsuario,
            Contrasena = usuario.Contrasena,
            RolId = idRol,
            Estado = usuario.Estado ? 1 : 0
        };

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, parametros, cancellationToken: cancellationToken));
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        using var conexion = _connectionFactory.CrearConexion();
        if (conexion.State != ConnectionState.Open) conexion.Open();

        const string sql = "DELETE FROM Usuarios WHERE id_usuario = @Id;";

        await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}