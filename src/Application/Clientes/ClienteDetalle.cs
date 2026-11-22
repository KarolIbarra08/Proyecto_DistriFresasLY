namespace DistriFresasLY.Application.Clientes;

public sealed record ClienteDetalle(
    int Id,
    string Nombre,
    string CedulaNit,
    string Telefono,
    string Direccion,
    string TipoNegocio,
    bool Activo);