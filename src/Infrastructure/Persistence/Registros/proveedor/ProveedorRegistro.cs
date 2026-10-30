using DistriFresasLY.Domain.Entities.Proveedores;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

public sealed class ProveedorRegistro
{
    public int Id_Proveedor { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Cedula_Nit { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Nombre_Empresa { get; set; } = string.Empty;

    public Proveedor ToEntidad()
    {
        return Proveedor.Reconstruir(
            Id_Proveedor,
            Nombre,
            Apellido,
            Cedula_Nit,
            Telefono,
            Direccion,
            Nombre_Empresa);
    }
}