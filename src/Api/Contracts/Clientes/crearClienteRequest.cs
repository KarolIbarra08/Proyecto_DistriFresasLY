namespace DistriFresasLY.Api.Contracts.Clientes;

public record CrearClienteRequest(
    string Nombre,
    string CedulaNit,
    string Telefono,
    string Direccion,
    string TipoNegocio
);