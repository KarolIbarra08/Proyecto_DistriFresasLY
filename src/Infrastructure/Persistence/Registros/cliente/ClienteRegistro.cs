using DistriFresasLY.Domain.Entities.Cliente;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

internal sealed class ClienteRegistro
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string CedulaNit { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Direccion { get; init; } = string.Empty;
    public string TipoNegocio { get; init; } = string.Empty;
    public bool Activo { get; init; }

    public Cliente ToEntidad() => Cliente.Rehidratar(
        Id,
        Nombre,
        CedulaNit,
        Telefono,
        Direccion,
        TipoNegocio,
        Activo);
}