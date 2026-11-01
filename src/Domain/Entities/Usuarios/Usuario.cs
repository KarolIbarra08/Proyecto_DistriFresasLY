using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Usuarios;

public sealed class Usuario : Entity
{
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string NombreUsuario { get; private set; }
    public string Contrasena { get; private set; }
    public string Rol { get; private set; }
    public bool Estado { get; private set; }

    private Usuario(
        int id,
        string nombre,
        string apellido,
        string usuario,
        string contrasena,
        string rol,
        bool estado) : base(id)
    {
        Nombre = nombre;
        Apellido = apellido;
        NombreUsuario = usuario;
        Contrasena = contrasena;
        Rol = rol;
        Estado = estado;
    }

    public static Result<Usuario> Create(
        string nombre,
        string apellido,
        string usuario,
        string contrasena,
        string rol,
        bool estado = true,
        int id = 0)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure<Usuario>(Error.Validation("Usuario.NombreObligatorio", "El nombre del usuario es obligatorio."));

        if (string.IsNullOrWhiteSpace(apellido))
            return Result.Failure<Usuario>(Error.Validation("Usuario.ApellidoObligatorio", "El apellido del usuario es obligatorio."));

        if (string.IsNullOrWhiteSpace(usuario))
            return Result.Failure<Usuario>(Error.Validation("Usuario.UsuarioObligatorio", "El nombre de usuario es obligatorio."));

        if (string.IsNullOrWhiteSpace(contrasena))
            return Result.Failure<Usuario>(Error.Validation("Usuario.ContrasenaObligatoria", "La contraseña es obligatoria."));

        if (string.IsNullOrWhiteSpace(rol))
            return Result.Failure<Usuario>(Error.Validation("Usuario.RolObligatorio", "El rol del usuario es obligatorio."));

        var usuarioCrear = new Usuario(
            id,
            nombre.Trim(),
            apellido.Trim(),
            usuario.Trim(),
            contrasena,
            rol.Trim(),
            estado);

        return Result.Success(usuarioCrear);
    }


    public Result ActualizarDatos(
        string nombre,
        string apellido,
        string usuario,
        string contrasena,
        string rol,
        bool estado)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure(Error.Validation("Usuario.NombreObligatorio", "El nombre del usuario es obligatorio."));

        if (string.IsNullOrWhiteSpace(apellido))
            return Result.Failure(Error.Validation("Usuario.ApellidoObligatorio", "El apellido del usuario es obligatorio."));

        if (string.IsNullOrWhiteSpace(usuario))
            return Result.Failure(Error.Validation("Usuario.UsuarioObligatorio", "El nombre de usuario es obligatorio."));

        if (string.IsNullOrWhiteSpace(contrasena))
            return Result.Failure(Error.Validation("Usuario.ContrasenaObligatoria", "La contraseña es obligatoria."));

        if (string.IsNullOrWhiteSpace(rol))
            return Result.Failure(Error.Validation("Usuario.RolObligatorio", "El rol del usuario es obligatorio."));

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        NombreUsuario = usuario.Trim();
        Contrasena = contrasena;
        Rol = rol.Trim();
        Estado = estado;

        return Result.Success();
    }

    public bool VerificarCredenciales(string usuarioIngresado, string contrasenaIngresada)
    {
        return NombreUsuario.Equals(usuarioIngresado, StringComparison.OrdinalIgnoreCase)
            && Contrasena == contrasenaIngresada;
    }
}