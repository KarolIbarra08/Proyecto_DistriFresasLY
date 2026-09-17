namespace DistriFresasLY.Api.Contracts.Clientes;

public record ClienteResponse(
    int Id,
    string Nombre,
    string CedulaNit,
    string Telefono,
    string Direccion,
    string TipoNegocio
);