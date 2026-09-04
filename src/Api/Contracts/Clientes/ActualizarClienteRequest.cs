namespace DistriFresasLY.Api.Contracts.Clientes;

public record ActualizarClienteRequest(
    string Nombre,
    string CedulaNit,
    string? Telefono,
    string? Direccion,
    string? TipoNegocio
);