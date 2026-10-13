using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Domain.Entities.Roles;

public sealed class Rol : Entity
{
    private readonly List<string> _permisos = [];

    public string Nombre { get; private set; }

    public string Descripcion { get; private set; }

    public IReadOnlyCollection<string> Permisos => _permisos.AsReadOnly();

    private Rol(int id, string nombre, string descripcion, List<string>? permisos) : base(id)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        
        if (permisos is not null)
        {
            foreach (var permiso in permisos)
            {
                AgregarPermiso(permiso); 
            }
        }
    }

    public static Result<Rol> Create(
        string nombre,
        string descripcion,
        List<string>? permisos = null,
        int id = 0)
    {

        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure<Rol>(
                Error.Validation(
                    "Rol.NombreObligatorio",
                    "El nombre del rol es obligatorio."));
        }

        
        if (string.IsNullOrWhiteSpace(descripcion))
        {
            return Result.Failure<Rol>(
                Error.Validation("Rol.DescripcionObligatoria", "La descripción del rol es obligatoria."));
        }

        var rol = new Rol(
            id,
            nombre.Trim(),
            descripcion?.Trim() ?? string.Empty,
            permisos);

        return Result.Success(rol);
    }

    public Result Actualizar(string nombre, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure(
                Error.Validation(
                    "Rol.NombreObligatorio",
                    "El nombre del rol es obligatorio."));
        }

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;

        return Result.Success();
    }

    public Result AgregarPermiso(string permiso)
    {
        if (string.IsNullOrWhiteSpace(permiso))
        {
            return Result.Failure(
                Error.Validation(
                    "Rol.PermisoVacio",
                    "El nombre del permiso es obligatorio."));
        }

        var permisoNormalizado = permiso.Trim().ToLower();

        if (!_permisos.Contains(permisoNormalizado))
        {
            _permisos.Add(permisoNormalizado);
        }

        return Result.Success();
    }

    public bool TieneAcceso(string permiso)
    {
        return !string.IsNullOrWhiteSpace(permiso)
            && _permisos.Contains(permiso.Trim().ToLower());
    }
}