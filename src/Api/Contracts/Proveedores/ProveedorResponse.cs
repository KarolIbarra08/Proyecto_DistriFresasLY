namespace DistriFresasLY.Api.Contracts.Proveedores;

public record ProveedorResponse(
    int Id,
    string Nombre,
    string Apellido,
    string CedulaNit,
    string Telefono,
    string Direccion,
    string NombreEmpresa
);