using DistriFresasLY.Application.Abstractions;

namespace DistriFresasLY.Infrastructure.Servicios;

/// <summary>
/// Implementación provisional de IUsuarioActual.
/// Mientras no exista autenticación, identifica al usuario como "sistema".
/// En el futuro podrá reemplazarse por una implementación basada en JWT.
/// </summary>
internal sealed class UsuarioActualSistema : IUsuarioActual
{
    public string Nombre => "sistema";
}