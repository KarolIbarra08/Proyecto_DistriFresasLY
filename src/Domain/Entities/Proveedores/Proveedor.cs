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
        string nombreEmpresa)
    {
        Id = id;
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
        // Validar nombre
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure<Proveedor>(
                Error.Validation(
                    "Proveedor.NombreObligatorio",
                    "El nombre del proveedor es obligatorio."));
        }

        nombre = nombre.Trim();

        if (nombre.Length < 3)
        {
            return Result.Failure<Proveedor>(
                Error.Validation(
                    "Proveedor.NombreInvalido",
                    "El nombre del proveedor debe tener al menos 3 caracteres."));
        }

        // Validar Cédula/NIT
        if (string.IsNullOrWhiteSpace(cedulaNit))
        {
            return Result.Failure<Proveedor>(
                Error.Validation(
                    "Proveedor.CedulaNitObligatoria",
                    "La Cedula/NIT es obligatoria."));
        }

        var documentoResult = CedulaNit.Create(cedulaNit);

        if (documentoResult.IsFailure)
        {
            return Result.Failure<Proveedor>(documentoResult.Error);
        }

        // Validar teléfono
        var telefonoResult = Telefono.Create(telefono);

        if (telefonoResult.IsFailure)
        {
            return Result.Failure<Proveedor>(telefonoResult.Error);
        }

        // Validar dirección
        if (string.IsNullOrWhiteSpace(direccion))
        {
            return Result.Failure<Proveedor>(
                Error.Validation(
                    "Proveedor.DireccionObligatoria",
                    "La direccion es obligatoria."));
        }

        direccion = direccion.Trim();

        // Crear proveedor
        var proveedor = new Proveedor(
            id,
            nombre,
            apellido?.Trim() ?? string.Empty,
            documentoResult.Value,
            telefonoResult.Value,
            direccion,
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
        // Validar nombre
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure(
                Error.Validation(
                    "Proveedor.NombreObligatorio",
                    "El nombre del proveedor es obligatorio."));
        }

        nombre = nombre.Trim();

        if (nombre.Length < 3)
        {
            return Result.Failure(
                Error.Validation(
                    "Proveedor.NombreInvalido",
                    "El nombre del proveedor debe tener al menos 3 caracteres."));
        }

        // Validar teléfono
        var telefonoResult = Telefono.Create(telefono);

        if (telefonoResult.IsFailure)
        {
            return Result.Failure(telefonoResult.Error);
        }

        // Validar dirección
        if (string.IsNullOrWhiteSpace(direccion))
        {
            return Result.Failure(
                Error.Validation(
                    "Proveedor.DireccionObligatoria",
                    "La direccion es obligatoria."));
        }

        direccion = direccion.Trim();

        // Actualizar
        Nombre = nombre;
        Apellido = apellido?.Trim() ?? string.Empty;
        Telefono = telefonoResult.Value;
        Direccion = direccion;
        NombreEmpresa = nombreEmpresa?.Trim() ?? string.Empty;

        return Result.Success();
    }

    public Result ActualizarDocumento(string nuevaCedulaNit)
    {
        if (string.IsNullOrWhiteSpace(nuevaCedulaNit))
        {
            return Result.Failure(
                Error.Validation(
                    "Proveedor.CedulaNitObligatoria",
                    "La Cedula/NIT es obligatoria."));
        }

        var docResult = CedulaNit.Create(nuevaCedulaNit);

        if (docResult.IsFailure)
        {
            return Result.Failure(docResult.Error);
        }

        Documento = docResult.Value;

        return Result.Success();
    }
}