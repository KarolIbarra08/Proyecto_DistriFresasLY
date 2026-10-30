using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Cliente;

public sealed class Cliente : Entity
{
    public string Nombre { get; private set; }
    public CedulaNit Documento { get; private set; }
    public Telefono Telefono { get; private set; }
    public string Direccion { get; private set; }
    public string TipoNegocio { get; private set; }
    public bool Activo { get; private set; }

    private Cliente(
        int id,
        string nombre,
        CedulaNit documento,
        Telefono telefono,
        string direccion,
        string tipoNegocio,
        bool activo)
    {
        Id = id; 
        Nombre = nombre;
        Documento = documento;
        Telefono = telefono;
        Direccion = direccion;
        TipoNegocio = tipoNegocio;
        Activo = activo;
    }

    public static Cliente Rehidratar(
        int id,
        string nombre,
        string cedulaNit,
        string telefono,
        string direccion,
        string tipoNegocio,
        bool activo)
    {
        var documento = CedulaNit.Rehidratar(cedulaNit);
        var telefonoObj = Telefono.Rehidratar(telefono);

        return new Cliente(
            id,
            nombre ?? string.Empty,
            documento,
            telefonoObj,
            direccion ?? string.Empty,
            tipoNegocio ?? string.Empty,
            activo);
    }

    public static Result<Cliente> Create(
        string nombre,
        string cedulaNit,
        string telefono,
        string direccion,
        string tipoNegocio,
        int id = 0)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure<Cliente>(
                Error.Validation("Cliente.NombreObligatorio", "El nombre del cliente es obligatorio."));
        }

        nombre = nombre.Trim();

        if (nombre.Length < 3)
        {
            return Result.Failure<Cliente>(
                Error.Validation("Cliente.NombreInvalido", "El nombre del cliente debe tener al menos 3 caracteres."));
        }

        if (string.IsNullOrWhiteSpace(cedulaNit))
        {
            return Result.Failure<Cliente>(
                Error.Validation("Cliente.CedulaNitObligatoria", "La Cédula/NIT es obligatoria."));
        }

        var documentoResult = CedulaNit.Create(cedulaNit);
        if (documentoResult.IsFailure)
        {
            return Result.Failure<Cliente>(documentoResult.Error);
        }

        var telefonoResult = Telefono.Create(telefono);
        if (telefonoResult.IsFailure)
        {
            return Result.Failure<Cliente>(telefonoResult.Error);
        }

        if (string.IsNullOrWhiteSpace(direccion))
        {
            return Result.Failure<Cliente>(
                Error.Validation("Cliente.DireccionObligatoria", "La dirección es obligatoria."));
        }

        direccion = direccion.Trim();

        if (string.IsNullOrWhiteSpace(tipoNegocio))
        {
            return Result.Failure<Cliente>(
                Error.Validation("Cliente.TipoNegocioObligatorio", "El tipo de negocio es obligatorio."));
        }

        tipoNegocio = tipoNegocio.Trim();

        var cliente = new Cliente(
            id,
            nombre,
            documentoResult.Value,
            telefonoResult.Value,
            direccion,
            tipoNegocio,
            true);

        return Result.Success(cliente);
    }

    public Result ActualizarDatos(
        string? nombre,
        string? telefono,
        string? direccion,
        string? tipoNegocio)
    {
    
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            nombre = nombre.Trim();

            if (nombre.Length < 3)
            {
                return Result.Failure(
                    Error.Validation("Cliente.NombreInvalido", "El nombre del cliente debe tener al menos 3 caracteres."));
            }

            Nombre = nombre;
        }

        if (!string.IsNullOrWhiteSpace(telefono))
        {
            var telefonoResult = Telefono.Create(telefono);
            if (telefonoResult.IsFailure)
            {
                return Result.Failure(telefonoResult.Error);
            }

            Telefono = telefonoResult.Value;
        }

        if (!string.IsNullOrWhiteSpace(direccion))
        {
            Direccion = direccion.Trim();
        }


        if (!string.IsNullOrWhiteSpace(tipoNegocio))
        {
            TipoNegocio = tipoNegocio.Trim();
        }

        return Result.Success();
    }

    public Result ActualizarDocumento(string? nuevaCedulaNit)
    {
        if (string.IsNullOrWhiteSpace(nuevaCedulaNit))
        {
            return Result.Success();
        }

        var docResult = CedulaNit.Create(nuevaCedulaNit);
        if (docResult.IsFailure)
        {
            return Result.Failure(docResult.Error);
        }

        Documento = docResult.Value;

        return Result.Success();
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}