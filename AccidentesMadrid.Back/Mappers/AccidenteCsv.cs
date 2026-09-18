using CsvHelper.Configuration.Attributes;

namespace AccidentesMadrid.Back.Mappers;

/// <summary>
///Conecta una columna del Csv con la propiedad dónde debe  guardar el valor
/// Acepta valores null porque ya limpiaremos esos campos en el mapper.
/// </summary>
public class AccidenteCsv
{
    [Name("num_expediente")]
    public string? NumExpediente { get; set; }

    [Name("fecha")]
    public string? Fecha { get; set; }

    [Name("hora")]
    public string? Hora { get; set; }

    [Name("localizacion")]
    public string? Localizacion { get; set; }

    [Name("numero")]
    public string? Numero { get; set; }

    [Name("cod_distrito")]
    public string? CodDistrito { get; set; }

    [Name("distrito")]
    public string? Distrito { get; set; }

    [Name("tipo_accidente")]
    public string? TipoAccidente { get; set; }

    [Name("estado_meteorológico")]
    public string? EstadoMeteorologico { get; set; }

    [Name("tipo_vehiculo")]
    public string? TipoVehiculo { get; set; }

    [Name("tipo_persona")]
    public string? TipoPersona { get; set; }

    [Name("rango_edad")]
    public string? RangoEdad { get; set; }

    [Name("sexo")]
    public string? Sexo { get; set; }

    [Name("cod_lesividad")]
    public string? CodLesividad { get; set; }

    [Name("lesividad")]
    public string? Lesividad { get; set; }

    [Name("coordenada_x_utm")]
    public string? CoordenadaXUtm { get; set; }

    [Name("coordenada_y_utm")]
    public string? CoordenadaYUtm { get; set; }

    [Name("positiva_alcohol")]
    public string? PositivaAlcohol { get; set; }

    [Name("positiva_droga")]
    public string? PositivaDroga { get; set; }
}