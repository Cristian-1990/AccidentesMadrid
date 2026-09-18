using AccidentesMadrid.Back.Errors.Common;

namespace AccidentesMadrid.Back.Errors;
/// <summary>
/// Clase abstracta que hereda de DomainErro y maneja los errores de 
/// </summary>
/// <param name="Message"></param>
public abstract record AccidenteError(string Message) : DomainError(Message)
{
    /// <summary>
    /// Error de dominio para cuando el fichero csv no existe.
    /// Detiene el programa por completo y no mostrará ningun resultado,
    /// mostrará el error
    /// </summary>
    /// <param name="Rutas"></param>
    public sealed record FicheroNoEncontrado(IEnumerable<string> Rutas)
        : AccidenteError($"No se encontraron los siguientes ficheros: {string.Join(", ", Rutas)}");
/// <summary>
/// Analiza cada fila y si una fila tiene datos erroneos captura el error
/// </summary>
/// <param name="NumExpediente">Identifica que número de fila fue la que falló para poder encontrarla</param>
/// <param name="Motivo">Explica porqué falló</param>
    public sealed record FilaInvalida(string NumExpediente, string Motivo)
        : AccidenteError($"Fila inválida ({NumExpediente}): {Motivo}");
}

/// <summary>
/// Factory para el manejo de errores
/// </summary>
///
///
/// ================ AÑADIMOS PATRON FACTCTORY PARA MANEJO DE ERRORES================
/// 
public static class AccidenteErrors
{
    /// <summary>
    /// Error para fichero no encontrado.
    /// Salta el error si alguno de los 3 ficheros csv necesario para arrancar da fallo
    /// </summary>
    /// <param name="rutas">ruta donde se encuentra del fichero que no se encontró</param>
    /// <returns>DomainErrror del tipo Fichero no encontrado</returns>
    public static DomainError FicheroNoEncontrado(IEnumerable<string> rutas) => new AccidenteError.FicheroNoEncontrado(rutas);
    /// <summary>
    /// Fallo de una fila del csv que no se puedo convertir al modelo
    /// No detiene el programa, descarta la fila y sigue adelante.
    /// </summary>
    /// <param name="numExpediente">identificador de la fila que falló</param>
    /// <param name="motivo">Explica el motivo de forma concreta</param>
    /// <returns>DomainError del tipo FilaInvalilda</returns>
    public static DomainError FilaInvalida(string numExpediente, string motivo) => new AccidenteError.FilaInvalida(numExpediente, motivo);
}