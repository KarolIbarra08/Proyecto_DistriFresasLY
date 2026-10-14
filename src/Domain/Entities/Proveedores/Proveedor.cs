using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Proveedores;

public sealed class Proveedor : Entity
{
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public CedulaNit Documento { get; private set; }
    public Telefono Telefono { get; private set; }
    public string Direccion { get; private set; }
    public string NombreEmpresa { get; private set; }

    private Proveedor(
        int id,
        string nombre,
        string apellido,
        CedulaNit documento,
        Telefono telefono,
        string direccion,
        string nombreEmpresa) : base(id)
    {
        Nombre = nombre;
        Apellido = apellido;
        Documento = documento;
        Telefono = telefono;
        Direccion = direccion;
        NombreEmpresa = nombreEmpresa;
    }

    public static Result<Proveedor> Create(
        string nombre,
        string apellido,
        string cedulaNit,
        string telefono,
        string direccion,
        string nombreEmpresa,
        int id = 0)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure<Proveedor>(
                Error.Validation("Proveedor.NombreObligatorio", "El nombre del proveedor es obligatorio."));
        }

        nombre = nombre.Trim();

        if (nombre.Length < 3)
        {
            return Result.Failure<Proveedor>(
                Error.Validation("Proveedor.NombreInvalido", "El nombre del proveedor debe tener al menos 3 caracteres."));
        }

        var documentoResult = CedulaNit.Create(cedulaNit);
        if (documentoResult.IsFailure)
        {
            return Result.Failure<Proveedor>(documentoResult.Error);
        }

        var telefonoResult = Telefono.Create(telefono);
        if (telefonoResult.IsFailure)
        {
            return Result.Failure<Proveedor>(telefonoResult.Error);
        }

        if (string.IsNullOrWhiteSpace(direccion))
        {
            return Result.Failure<Proveedor>(
                Error.Validation("Proveedor.DireccionObligatoria", "La dirección es obligatoria."));
        }

        var proveedor = new Proveedor(
            id,
            nombre,
            apellido?.Trim() ?? string.Empty,
            documentoResult.Value,
            telefonoResult.Value,
            direccion.Trim(),
            nombreEmpresa?.Trim() ?? string.Empty);

        return Result.Success(proveedor);
    }

    public Result ActualizarDatos(
        string nombre,
        string apellido,
        string telefono,
        string direccion,
        string nombreEmpresa)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure(
                Error.Validation("Proveedor.NombreObligatorio", "El nombre del proveedor es obligatorio."));
        }

        nombre = nombre.Trim();

        if (nombre.Length < 3)
        {
            return Result.Failure(
                Error.Validation("Proveedor.NombreInvalido", "El nombre del proveedor debe tener al menos 3 caracteres."));
        }

        var telefonoResult = Telefono.Create(telefono);
        if (telefonoResult.IsFailure)
        {
            return Result.Failure(telefonoResult.Error);
        }

        if (string.IsNullOrWhiteSpace(direccion))
        {
            return Result.Failure(
                Error.Validation("Proveedor.DireccionObligatoria", "La dirección es obligatoria."));
        }

        Nombre = nombre;
        Apellido = apellido?.Trim() ?? string.Empty;
        Telefono = telefonoResult.Value;
        Direccion = direccion.Trim();
        NombreEmpresa = nombreEmpresa?.Trim() ?? string.Empty;

        return Result.Success();
    }

    public Result ActualizarDocumento(string nuevaCedulaNit)
    {
        var docResult = CedulaNit.Create(nuevaCedulaNit);
        if (docResult.IsFailure)
        {
            return Result.Failure(docResult.Error);
        }

        Documento = docResult.Value;
        return Result.Success();
    }
}