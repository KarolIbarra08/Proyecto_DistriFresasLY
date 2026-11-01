namespace DistriFresasLY.Domain.Common;

/// <summary>
/// Excepción para conflictos de negocio que deben responder con HTTP 409.
/// </summary>
public sealed class ConflictoException : Exception
{
    public string Code { get; }

    public ConflictoException(string code, string message) : base(message)
    {
        Code = code;
    }
}
