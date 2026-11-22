namespace DistriFresasLY.Application.Proveedores;

public sealed record ProveedorDetalle(
    int Id,
    string Nombre,
    string Apellido,
    string CedulaNit,
    string Telefono,
    string Direccion,
    string NombreEmpresa);