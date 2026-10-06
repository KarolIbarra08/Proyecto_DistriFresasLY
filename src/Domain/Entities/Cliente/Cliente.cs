using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.ValueObjects;

namespace DistriFresasLY.Domain.Entities.Cliente;


public sealed class Cliente : Entity
{
    public string Nombre { get; private set; }
    public CedulaNit Documento { get; private set; }
    public string Telefono { get; private set; }
    public string Direccion { get; private set; }
    public string TipoNegocio { get; private set; }
    public bool Activo { get; private set; }


    private Cliente(
        int id,
        string nombre,
        CedulaNit documento,
        string telefono,
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

    
    public static Result<Cliente> Create(
        string nombre,
        string cedulaNit,
        string? telefono = null,
        string? direccion = null,
        string? tipoNegocio = null,
        int id = 0)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length < 3)
        {
            return Error.Validation(
                "Cliente.NombreInvalido",
                "El nombre del cliente es obligatorio y debe tener al menos 3 caracteres.");
        }

        
        var documentoResult = ValueObjects.CedulaNit.Create(cedulaNit);
        if (documentoResult.IsFailure)
        {
            return documentoResult.Error;
        }

        var cliente = new Cliente(
            id,
            nombre.Trim(),
            documentoResult.Value,
            telefono?.Trim() ?? string.Empty,
            direccion?.Trim() ?? string.Empty,
            tipoNegocio?.Trim() ?? string.Empty,
            activo: true);

        return cliente;
    }

 
    public Result ActualizarDatos(
        string nombre,
        string? telefono,
        string? direccion,
        string? tipoNegocio)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length < 3)
        {
            var error = Error.Validation(
                "Cliente.NombreInvalido",
                "El nombre del cliente no puede estar vacío y debe tener al menos 3 caracteres.");

            return Result.Failure(error);
        }

        Nombre = nombre.Trim();
        Telefono = telefono?.Trim() ?? string.Empty;
        Direccion = direccion?.Trim() ?? string.Empty;
        TipoNegocio = tipoNegocio?.Trim() ?? string.Empty;

        return Result.Success();
    }

  
    public Result ActualizarDocumento(string nuevaCedulaNit)
    {
        var docResult = ValueObjects.CedulaNit.Create(nuevaCedulaNit);
        if (docResult.IsFailure)
        {
            return Result.Failure(docResult.Error);
        }

        Documento = docResult.Value;
        return Result.Success();
    }

    public void Desactivar() => Activo = false;

    public void Activar() => Activar();
}