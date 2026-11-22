using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Domain.Entities.Proveedores;

namespace DistriFresasLY.Application.Proveedores;

public sealed record CrearProveedorCommand(
    string Nombre,
    string Apellido,
    string CedulaNit,
    string Telefono,
    string Direccion,
    string NombreEmpresa);

public sealed class CrearProveedorUseCase
{
    private readonly IProveedorRepository _proveedores;

    public CrearProveedorUseCase(IProveedorRepository proveedores)
    {
        _proveedores = proveedores;
    }

    public async Task<Result<ProveedorDetalle>> EjecutarAsync(
        CrearProveedorCommand comando,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(comando.Nombre) || string.IsNullOrWhiteSpace(comando.CedulaNit))
        {
            return Result.Failure<ProveedorDetalle>(ProveedorErrores.DatosObligatorios);
        }

        var proveedorExistente = await _proveedores.ObtenerPorCedulaNitAsync(comando.CedulaNit, cancellationToken);
        if (proveedorExistente is not null)
        {
            return Result.Failure<ProveedorDetalle>(ProveedorErrores.CedulaDuplicada(comando.CedulaNit));
        }

        var proveedorResult = Proveedor.Create(
            nombre: comando.Nombre,
            apellido: comando.Apellido,
            cedulaNit: comando.CedulaNit,
            telefono: comando.Telefono,
            direccion: comando.Direccion,
            nombreEmpresa: comando.NombreEmpresa);

        if (proveedorResult.IsFailure)
        {
            return Result.Failure<ProveedorDetalle>(proveedorResult.Error);
        }

        var proveedor = proveedorResult.Value;
        var nuevoId = await _proveedores.AgregarAsync(proveedor, cancellationToken);

        return Result.Success(new ProveedorDetalle(
            nuevoId,
            proveedor.Nombre,
            proveedor.Apellido,
            proveedor.Documento.Value,
            proveedor.Telefono.Valor,
            proveedor.Direccion,
            proveedor.NombreEmpresa));
    }
}