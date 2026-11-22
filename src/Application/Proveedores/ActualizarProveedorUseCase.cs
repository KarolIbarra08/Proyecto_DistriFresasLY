using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Proveedores;

public sealed record ActualizarProveedorCommand(
    int Id,
    string? Nombre,
    string? Apellido,
    string? CedulaNit,
    string? Telefono,
    string? Direccion,
    string? NombreEmpresa);

public sealed class ActualizarProveedorUseCase
{
    private readonly IProveedorRepository _proveedores;

    public ActualizarProveedorUseCase(IProveedorRepository proveedores)
    {
        _proveedores = proveedores;
    }

    public async Task<Result<ProveedorDetalle>> EjecutarAsync(
        ActualizarProveedorCommand comando,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _proveedores.ObtenerPorIdAsync(comando.Id, cancellationToken);
        if (proveedor is null)
        {
            return Result.Failure<ProveedorDetalle>(ProveedorErrores.NoEncontrado(comando.Id));
        }

        if (!string.IsNullOrWhiteSpace(comando.CedulaNit) &&
            !comando.CedulaNit.Trim().Equals(proveedor.Documento.Value, StringComparison.OrdinalIgnoreCase))
        {
            var duplicado = await _proveedores.ObtenerPorCedulaNitAsync(comando.CedulaNit, cancellationToken);
            if (duplicado is not null && duplicado.Id != comando.Id)
            {
                return Result.Failure<ProveedorDetalle>(ProveedorErrores.CedulaDuplicada(comando.CedulaNit));
            }

            var documentoResult = proveedor.ActualizarDocumento(comando.CedulaNit.Trim());
            if (documentoResult.IsFailure)
            {
                return Result.Failure<ProveedorDetalle>(documentoResult.Error);
            }
        }

        var nombre = !string.IsNullOrWhiteSpace(comando.Nombre) ? comando.Nombre.Trim() : proveedor.Nombre;
        var apellido = !string.IsNullOrWhiteSpace(comando.Apellido) ? comando.Apellido.Trim() : proveedor.Apellido;
        var telefono = !string.IsNullOrWhiteSpace(comando.Telefono) ? comando.Telefono.Trim() : proveedor.Telefono.Valor;
        var direccion = !string.IsNullOrWhiteSpace(comando.Direccion) ? comando.Direccion.Trim() : proveedor.Direccion;
        var nombreEmpresa = !string.IsNullOrWhiteSpace(comando.NombreEmpresa) ? comando.NombreEmpresa.Trim() : proveedor.NombreEmpresa;

        var actualizarResult = proveedor.ActualizarDatos(nombre, apellido, telefono, direccion, nombreEmpresa);
        if (actualizarResult.IsFailure)
        {
            return Result.Failure<ProveedorDetalle>(actualizarResult.Error);
        }

        await _proveedores.ActualizarAsync(proveedor, cancellationToken);

        return Result.Success(new ProveedorDetalle(
            proveedor.Id,
            proveedor.Nombre,
            proveedor.Apellido,
            proveedor.Documento.Value,
            proveedor.Telefono.Valor,
            proveedor.Direccion,
            proveedor.NombreEmpresa));
    }
}