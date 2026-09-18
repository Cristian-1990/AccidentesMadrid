using AccidentesMadrid.Back.Errors;
using AccidentesMadrid.Back.Errors.Common;
using AccidentesMadrid.Back.Models;
using CSharpFunctionalExtensions;

namespace AccidentesMadrid.Back.Mappers;
/// <summary>
/// Agrupa y divide las filas del Csv en modelos del dominio
/// </summary>
public static class AccidenteMapper
{
    /// <summary>
    /// Convierte, valida y descarta la fila  de un csv en atributos de la clase del dominio.
    /// Si un campo falla en la conversion descarta la fila y devuelve un DomainError
    /// </summary>
    /// <param name="fila">La fila leida del csv sin procesar</param>
    /// <returns>Si todos los campos obligatorios son válidos devuelve un Accidente,
    /// si no lo son devuelve el error de la conversion de esa fila</returns>
    public static Result<Accidente, DomainError> ToAccidente(this AccidenteCsv fila)
    {
        bool? positivaAlcohol = fila.PositivaAlcohol
            switch
            {
                "S" => true,
                "N" => false,
                _ => null
            };
        bool? positivaDroga = fila.PositivaDroga
            switch
            {
                "1" => true,
                "" => false,
                _ => null
            };
        if (!DateOnly.TryParseExact(fila.Fecha, "dd/MM/yyyy", out var fecha))
            return Result.Failure<Accidente, DomainError>(
                AccidenteErrors.FilaInvalida(fila.NumExpediente ?? "desconocido", "fecha con formato incorrecto"));
        if (!TimeOnly.TryParse(fila.Hora, out var hora))
            return Result.Failure<Accidente, DomainError>(
                AccidenteErrors.FilaInvalida(fila.NumExpediente ?? "desconocido", "hora con formato incorrecto"));
        double? coordenadaXUtm = double.TryParse(fila.CoordenadaXUtm, out var x) ? x : null;
        double? coordenadaYUtm = double.TryParse(fila.CoordenadaYUtm, out var y) ? y : null;
        int? codLesividad = int.TryParse(fila.CodLesividad, out var cl) ? cl : null;
        
        if (!int.TryParse(fila.CodDistrito, out var codDistrito))
            return Result.Failure<Accidente, DomainError>(AccidenteErrors.FilaInvalida(fila.NumExpediente ?? "desconocido", "código de distrito no es un número válido"));
        
        var accidente = new Accidente
        {
            
            PositivaAlcohol = positivaAlcohol,
            PositivaDroga = positivaDroga,
            CodDistrito = codDistrito,
            CodLesividad = codLesividad,
            CoordenadaYUtm =coordenadaYUtm,
            CoordenadaXUtm = coordenadaXUtm,
            Fecha = fecha,
            Hora = hora,
            NumExpediente = fila.NumExpediente ?? string.Empty,
            Localizacion = fila.Localizacion ?? string.Empty,
            Distrito = fila.Distrito ?? string.Empty,
            TipoAccidente = fila.TipoAccidente ?? string.Empty,
            TipoVehiculo = fila.TipoVehiculo ?? string.Empty,
            TipoPersona = fila.TipoPersona ?? string.Empty,
            RangoEdad = fila.RangoEdad ?? string.Empty,
            EstadoMeteorologico = fila.EstadoMeteorologico,
            Numero = fila.Numero,
            Sexo = fila.Sexo,
            Lesividad = fila.Lesividad,
        };
        return Result.Success<Accidente, DomainError>(accidente);
    }
}
