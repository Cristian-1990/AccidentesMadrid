using AccidentesMadrid.Back.Errors.Common;

namespace AccidentesMadrid.Back.Errors;
/// <summary>
/// 
/// </summary>
/// <param name="Message"></param>
public abstract record AccidenteError(string Message) : DomainError(Message)
{
    public sealed record FicheroNoEncontrado(IEnumerable<string> Rutas)
        : AccidenteError($"No se encontraron los siguientes ficheros: {string.Join(", ", Rutas)}");

    public sealed record FilaInvalida(string NumExpediente, string Motivo)
        : AccidenteError($"Fila inválida ({NumExpediente}): {Motivo}");
}

public static class AccidenteErrors
{
    public static DomainError FicheroNoEncontrado(IEnumerable<string> rutas) => new AccidenteError.FicheroNoEncontrado(rutas);
    public static DomainError FilaInvalida(string numExpediente, string motivo) => new AccidenteError.FilaInvalida(numExpediente, motivo);
}