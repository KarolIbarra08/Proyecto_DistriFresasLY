namespace DistriFresasLY.Api.Contracts.Proveedores;

public record ActualizarProveedorRequest(
    string Nombre,
    string Apellido,
    string CedulaNit,
    string? Telefono,
    string? Direccion,
    string? NombreEmpresa
);