namespace AccidentesMadrid.Back.Models;

/// <summary>
/// Entidad que representa un accidente de trafico.
/// Record para comparar los valores del accidente y no su posicion en memoria (class)
/// </summary>
public record Accidente
{
    public string NumExpediente { get; init; } = string.Empty;
    public DateOnly Fecha { get; init; }
    public TimeOnly Hora { get; init; }
    public string Localizacion { get; init; } = string.Empty;
    public string? Numero { get; init; }
    public int CodDistrito { get; init; }
    public string Distrito { get; init; } = string.Empty;
    public string TipoAccidente { get; init; } = string.Empty;
    public string? EstadoMeteorologico { get; init; }
    public string TipoVehiculo { get; init; } = string.Empty;
    public string TipoPersona { get; init; } = string.Empty;
    public string RangoEdad { get; init; } = string.Empty;
    public string? Sexo { get; init; }
    public int? CodLesividad { get; init; }
    public string? Lesividad { get; init; }
    public double? CoordenadaXUtm { get; init; }
    public double? CoordenadaYUtm { get; init; }
    public bool? PositivaAlcohol { get; init; }
    public bool? PositivaDroga { get; init; }
}