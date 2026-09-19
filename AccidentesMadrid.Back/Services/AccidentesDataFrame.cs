using Microsoft.Data.Analysis;

namespace AccidentesMadrid.Back.Services;

public class AccidentesDataFrame(DataFrame df)
{
    private static bool EsPositivo(string? valor) => valor == "S";

    public int TotalAccidentes() =>
        df["num_expediente"].Cast<string?>().Distinct().Count();
    public int AccidentesConPeatones() =>
        ParsearFilas().Where(f => f.TipoPersona == "Peatón").Select(f => f.Expediente).Distinct().Count();

    public int PositivosEnAlcohol()
    {
        return df["positiva_alcohol"]
            .Cast<string?>()
            .Count(EsPositivo);
    }
    
    private List<(string Clave, int Total)> ContarPorColumnaAccidente(string columna)
    {
        var claves = df[columna].Cast<string?>();
        var expedientes = df["num_expediente"].Cast<string?>();

        return claves
            .Zip(expedientes, (clave, exp) => (Clave: clave ?? "Desconocido", Expediente: exp))
            .DistinctBy(x => x.Expediente)
            .GroupBy(x => x.Clave)
            .Select(g => (Clave: g.Key, Total: g.Count()))
            .ToList();
    }
    private List<(string Clave, int Total)> ContarPorColumna(string columna)
    {
        return df[columna]
            .Cast<string?>()
            .GroupBy(v => v ?? "Desconocido")
            .Select(g => (Clave: g.Key, Total: g.Count()))
            .ToList();
    }
    private List<(string Clave, int Total)> ContarFiltradoPorColumna(
        string columnaFiltro, Func<string?, bool> condicion, string columnaAgrupar)
    {
        var filtro = df[columnaFiltro].Cast<string?>();
        var claves = df[columnaAgrupar].Cast<string?>();

        return filtro
            .Zip(claves, (f, c) => (Filtro: f, Clave: c ?? "Desconocido"))
            .Where(x => condicion(x.Filtro))
            .GroupBy(x => x.Clave)
            .Select(g => (Clave: g.Key, Total: g.Count()))
            .ToList();
    }
    private record FilaParseada(
        string? Expediente, DateOnly? Fecha, TimeOnly? Hora,
        string Distrito, string Lesividad, string? TipoPersona, string? PositivaAlcohol);

    private static DateOnly? ParsearFecha(string? valor) =>
        DateOnly.TryParseExact(valor, "dd/MM/yyyy", out var f) ? f : null;

    private static TimeOnly? ParsearHora(string? valor) =>
        TimeOnly.TryParse(valor, out var h) ? h : null;

    private List<FilaParseada>? _filasCache;

    private List<FilaParseada> ParsearFilas()
    {
        if (_filasCache is not null) return _filasCache;

        var expedientes = df["num_expediente"].Cast<string?>().ToList();
        var fechas = df["fecha"].Cast<string?>().ToList();
        var horas = df["hora"].Cast<string?>().ToList();
        var distritos = df["distrito"].Cast<string?>().ToList();
        var lesividades = df["lesividad"].Cast<string?>().ToList();
        var tiposPersona = df["tipo_persona"].Cast<string?>().ToList();
        var alcohol = df["positiva_alcohol"].Cast<string?>().ToList();

        var filas = new List<FilaParseada>();
        for (int i = 0; i < expedientes.Count; i++)
        {
            filas.Add(new FilaParseada(
                expedientes[i], ParsearFecha(fechas[i]), ParsearHora(horas[i]),
                distritos[i] ?? "Desconocido", lesividades[i] ?? "Desconocido",
                tiposPersona[i], alcohol[i]));
        }

        _filasCache = filas;
        return filas;
    }

    private List<FilaParseada> AccidentesUnicos() =>
        ParsearFilas().Where(f => f.Fecha.HasValue).DistinctBy(f => f.Expediente).ToList();

    public List<(string Sexo, int Total)> AccidentesPorSexo() =>
        ContarPorColumna("sexo");

    public List<(string RangoEdad, int Total)> AccidentesPorRangoEdad() =>
        ContarPorColumna("rango_edad");

    public List<(string Distrito, int Total)> AccidentesPorDistritoTop5() =>
        ContarPorColumnaAccidente("distrito").OrderByDescending(x => x.Total).Take(5).ToList();

    public List<(string Tipo, int Total)> AccidentesPorTipo() =>
        ContarPorColumnaAccidente("tipo_accidente");

    public List<(string EstadoMeteorologico, int Total)> AccidentesPorEstadoMeteorologico() =>
        ContarPorColumnaAccidente("estado_meteorológico");

    public List<(int CodDistrito, int Total)> AccidentesPorCodDistrito() =>
        ContarPorColumnaAccidente("cod_distrito")
            .Select(x => (int.Parse(x.Clave), x.Total))
            .ToList();
  
    public int PositivosEnDrogas() =>
        df["positiva_droga"].Cast<string?>().Count(v => v == "1");

    public List<(string Lesividad, int Total)> LesionesMasFrecuentes() =>
        ContarPorColumna("lesividad").OrderByDescending(x => x.Total).ToList();

    public string TipoVehiculoMasImplicado() =>
        ContarPorColumna("tipo_vehiculo").OrderByDescending(x => x.Total).First().Clave;

    public (double Hombres, double Mujeres) ProporcionHombreMujer()
    {
        var sexos = df["sexo"].Cast<string?>().ToList();
        int total = sexos.Count(v => v == "Hombre" || v == "Mujer");
        int hombres = sexos.Count(v => v == "Hombre");
        int mujeres = sexos.Count(v => v == "Mujer");

        return (
            total == 0 ? 0 : (double)hombres / total * 100,
            total == 0 ? 0 : (double)mujeres / total * 100
        );
    }

    public List<(string Distrito, int TotalPeatones)> DistritosConMasPeatones() =>
        ContarFiltradoPorColumna("tipo_persona", v => v == "Peatón", "distrito")
            .OrderByDescending(x => x.Total).ToList();

    public int AccidentesConAlcoholYDroga()
    {
        var alcohol = df["positiva_alcohol"].Cast<string?>().ToList();
        var droga = df["positiva_droga"].Cast<string?>().ToList();
        return Enumerable.Range(0, alcohol.Count).Count(i => alcohol[i] == "S" && droga[i] == "1");
    }

    public List<(string RangoEdad, int Total)> RangosEdadMasVulnerablesPeatones() =>
        ContarFiltradoPorColumna("tipo_persona", v => v == "Peatón", "rango_edad")
            .OrderByDescending(x => x.Total).ToList();

    public List<(string Distrito, int Total)> DistritosConMasPositivosAlcohol() =>
        ContarFiltradoPorColumna("positiva_alcohol", v => v == "S", "distrito")
            .OrderByDescending(x => x.Total).ToList();
    public List<(DayOfWeek DiaSemana, int Total)> AccidentesPorDiaSemana() =>
    AccidentesUnicos().GroupBy(f => f.Fecha!.Value.DayOfWeek).Select(g => (g.Key, g.Count())).ToList();

public List<(int Mes, int Total)> AccidentesPorMes() =>
    AccidentesUnicos().GroupBy(f => f.Fecha!.Value.Month).Select(g => (g.Key, g.Count())).ToList();

public int HoraConMasAccidentes() =>
    AccidentesUnicos().Where(f => f.Hora.HasValue)
        .GroupBy(f => f.Hora!.Value.Hour).OrderByDescending(g => g.Count()).First().Key;

public (int FinDeSemana, int EntreSemana) FinDeSemanaVsEntreSemana()
{
    var unicos = AccidentesUnicos();
    int finDeSemana = unicos.Count(f => f.Fecha!.Value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
    return (finDeSemana, unicos.Count - finDeSemana);
}

public double MediaAccidentesPorDia() =>
    AccidentesUnicos().GroupBy(f => f.Fecha!.Value).Select(g => g.Count()).Average();

public List<(int Anio, int Total)> AccidentesPorAnio() =>
    AccidentesUnicos().GroupBy(f => f.Fecha!.Value.Year).Select(g => (g.Key, g.Count())).ToList();

public List<(int Anio, int Mes, int Total)> EvolucionMensualPorAnio() =>
    AccidentesUnicos()
        .GroupBy(f => new { f.Fecha!.Value.Year, f.Fecha!.Value.Month })
        .Select(g => (Anio: g.Key.Year, Mes: g.Key.Month, Total: g.Count()))
        .OrderBy(x => x.Anio).ThenBy(x => x.Mes).ToList();

public List<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio() =>
    AccidentesUnicos()
        .GroupBy(f => f.Fecha!.Value.Year)
        .Select(grupoAnio =>
        {
            var top = grupoAnio.GroupBy(f => f.Distrito).OrderByDescending(g => g.Count()).First();
            return (Anio: grupoAnio.Key, Distrito: top.Key, Total: top.Count());
        })
        .OrderBy(x => x.Anio).ToList();

public List<(int Anio, int FinDeSemana, int EntreSemana)> ComparativaFinDeSemanaEntreSemanaPorAnio() =>
    AccidentesUnicos()
        .GroupBy(f => f.Fecha!.Value.Year)
        .Select(g =>
        {
            int fds = g.Count(f => f.Fecha!.Value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
            return (Anio: g.Key, FinDeSemana: fds, EntreSemana: g.Count() - fds);
        })
        .OrderBy(x => x.Anio).ToList();

public List<(int Anio, int Hora, int Total)> HoraPicoPorAnio() =>
    AccidentesUnicos().Where(f => f.Hora.HasValue)
        .GroupBy(f => f.Fecha!.Value.Year)
        .Select(grupoAnio =>
        {
            var top = grupoAnio.GroupBy(f => f.Hora!.Value.Hour).OrderByDescending(g => g.Count()).First();
            return (Anio: grupoAnio.Key, Hora: top.Key, Total: top.Count());
        })
        .OrderBy(x => x.Anio).ToList();

// las siguientes 3 NO deduplican por expediente (mismo motivo que en AccidentesLinq: lesividad/peatón/alcohol son de la persona)

public List<(int Anio, string Lesividad, int Total)> LesionMasFrecuentePorAnio() =>
    ParsearFilas().Where(f => f.Fecha.HasValue)
        .GroupBy(f => f.Fecha!.Value.Year)
        .Select(grupoAnio =>
        {
            var top = grupoAnio.GroupBy(f => f.Lesividad).OrderByDescending(g => g.Count()).First();
            return (Anio: grupoAnio.Key, Lesividad: top.Key, Total: top.Count());
        })
        .OrderBy(x => x.Anio).ToList();

public List<(int Anio, int Total)> TendenciaAlcoholPorAnio() =>
    ParsearFilas().Where(f => f.Fecha.HasValue && f.PositivaAlcohol == "S")
        .GroupBy(f => f.Fecha!.Value.Year)
        .Select(g => (Anio: g.Key, Total: g.Count()))
        .OrderBy(x => x.Anio).ToList();

public List<(int Anio, int Total)> EvolucionPeatonesPorAnio() =>
    ParsearFilas().Where(f => f.Fecha.HasValue && f.TipoPersona == "Peatón")
        .GroupBy(f => f.Fecha!.Value.Year)
        .Select(g => (Anio: g.Key, Total: g.Count()))
        .OrderBy(x => x.Anio).ToList();
}
