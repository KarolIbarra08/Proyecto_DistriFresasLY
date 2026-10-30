using DistriFresasLY.Domain.Entities.Cliente;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

internal sealed class ClienteRegistro
{
    public int Id_Cliente { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Cedula_Nit { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Direccion { get; init; } = string.Empty;
    public string Tipo_De_Negocio { get; init; } = string.Empty;
    public bool Activo { get; init; }

    public Cliente ToEntidad() => Cliente.Rehidratar(
        Id_Cliente,
        Nombre,
        Cedula_Nit,
        Telefono,
        Direccion,
        Tipo_De_Negocio,
        Activo);
}