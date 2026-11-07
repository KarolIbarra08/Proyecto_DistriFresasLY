namespace DistriFresasLY.Application.Abstractions;

/// <summary>
/// Identifica al usuario que realiza una operación.
/// Se utiliza para registrar los datos de auditoría.
/// Mientras no exista autenticación, se utiliza el usuario "sistema".
/// Cuando se implemente JWT, se podrá cambiar su implementación.
/// </summary>
public interface IUsuarioActual
{
    string Nombre { get; }
}