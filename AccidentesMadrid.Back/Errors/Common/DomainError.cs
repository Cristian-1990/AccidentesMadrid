namespace AccidentesMadrid.Back.Errors.Common;
/// <summary>
/// Clase abstracta base que maneja cualquier error de dominio.
/// Son las clases hijas las que definen el tipo de error
/// </summary>
public abstract record DomainError
{
    /// <summary>
    /// Contiene el mensaje descriptivo del error
    /// </summary>
    public string Message { get; init; } = string.Empty;
/// <summary>
/// Constructor de la clase.
/// Protected para que solo las clases hijas puedan invocarlo
/// </summary>
/// <param name="message">Mensaje que describe el tipo de error</param>
    protected DomainError(string message)
    {
        Message = message;
    }
}