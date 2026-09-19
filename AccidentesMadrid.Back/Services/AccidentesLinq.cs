using AccidentesMadrid.Back.Models;
using AccidentesMadrid.Back.Repositories;
namespace AccidentesMadrid.Back.Services;

public class AccidentesLinq(List<Accidente> accidentes)
{
    public int TotalAccidentes()
    {
        var resultado = accidentes.DistinctBy(a => a.NumExpediente).Count();
        return resultado;
    }

    public List<(string Distrito, int Total)> AccidentesPorDistritoTop5()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Distrito)
            .Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToList();
        return resultado;
    }

    public List<(string Tipo, int Total)> AccidentesPorTipo()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.TipoAccidente)
            .Select(g => (Tipo: g.Key, Total: g.Count()))
            .ToList();
        return resultado;
    }

    public List<(string EstadoMeteorologico, int Total)> AccidentesPorEstadoMeteorologico()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.EstadoMeteorologico ?? "Desconocido")
            .Select(g => (EstadoMeteorologico: g.Key, Total: g.Count()))
            .ToList();
        return resultado;
    }

    public List<(string Sexo, int Total)> AccidentesPorSexo()
    {
        var resultado = accidentes
            .GroupBy(a => a.Sexo ?? "Desconocido")
            .Select(g => (Sexo: g.Key, Total: g.Count()))
            .ToList();
        return resultado;
    }

    public List<(string RangoEdad, int Total)> AccidentesPorRangoEdad()
    {
        var resultado = accidentes
            .GroupBy(a => a.RangoEdad ?? "Desconocido")
            .Select(g => (RangoEdad: g.Key, Total: g.Count()))
            .ToList();
        return resultado;
    }

    public int PositivosEnAlcohol()
    {
        var resultado = accidentes.Count(a => a.PositivaAlcohol == true);
        return resultado;
    }

    public int PositivosEnDrogas()
    {
        var resultado = accidentes.Count(a => a.PositivaDroga == true);
        return resultado;
    }

    public List<(DayOfWeek DiaSemana, int Total)> AccidentesPorDiaSemana()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.DayOfWeek)
            .Select(g => (DiaSemana: g.Key, Total: g.Count()))
            .ToList();
        return resultado;
    }

    public List<(int Mes, int Total)> AccidentesPorMes()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.Month)
            .Select(g => (Mes: g.Key, Total: g.Count()))
            .ToList();
        return resultado;
    }

    public int HoraConMasAccidentes()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Hora.Hour)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
        return resultado;
    }

    public List<(string Lesividad, int Total)> LesionesMasFrecuentes()
    {
        var resultado = accidentes
            .GroupBy(a => a.Lesividad ?? "Desconocido")
            .Select(g => (Lesividad: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total)
            .ToList();
        return resultado;
    }

    public string TipoVehiculoMasImplicado()
    {
        var resultado = accidentes
            .GroupBy(a => a.TipoVehiculo ?? "Desconocido")
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
        return resultado;
    }

    public int AccidentesConPeatones()
    {
        var resultado = accidentes
            .Where(a => a.TipoPersona == "Peatón")
            .DistinctBy(a => a.NumExpediente)
            .Count();
        return resultado;
    }

    public (double Hombres, double Mujeres) ProporcionHombreMujer()
    {
        int total = accidentes.Count(a => a.Sexo == "Hombre" || a.Sexo == "Mujer");
        int hombres = accidentes.Count(a => a.Sexo == "Hombre");
        int mujeres = accidentes.Count(a => a.Sexo == "Mujer");

        var resultado = (
            Hombres: total == 0 ? 0 : (double)hombres / total * 100,
            Mujeres: total == 0 ? 0 : (double)mujeres / total * 100
        );
        return resultado;
    }

    public List<(string Distrito, int TotalPeatones)> DistritosConMasPeatones()
    {
        var resultado = accidentes
            .Where(a => a.TipoPersona == "Peatón")
            .GroupBy(a => a.Distrito ?? "Desconocido")
            .Select(g => (Distrito: g.Key, TotalPeatones: g.Count()))
            .OrderByDescending(x => x.TotalPeatones)
            .ToList();
        return resultado;
    }

    public (int FinDeSemana, int EntreSemana) FinDeSemanaVsEntreSemana()
    {
        var accidentesUnicos = accidentes.DistinctBy(a => a.NumExpediente).ToList();

        int finDeSemana = accidentesUnicos.Count(a => a.Fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
        int entreSemana = accidentesUnicos.Count - finDeSemana;

        var resultado = (finDeSemana, entreSemana);
        return resultado;
    }

    public double MediaAccidentesPorDia()
    {
        var accidentesUnicos = accidentes.DistinctBy(a => a.NumExpediente);

        var resultado = accidentesUnicos
            .GroupBy(a => a.Fecha)
            .Select(g => g.Count())
            .Average();
        return resultado;
    }

    public int AccidentesConAlcoholYDroga()
    {
        var resultado = accidentes.Count(a => a.PositivaAlcohol == true && a.PositivaDroga == true);
        return resultado;
    }

    public List<(string RangoEdad, int Total)> RangosEdadMasVulnerablesPeatones()
    {
        var resultado = accidentes
            .Where(a => a.TipoPersona == "Peatón")
            .GroupBy(a => a.RangoEdad ?? "Desconocido")
            .Select(g => (RangoEdad: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total)
            .ToList();
        return resultado;
    }

    public List<(string Distrito, int Total)> DistritosConMasPositivosAlcohol()
    {
        var resultado = accidentes
            .Where(a => a.PositivaAlcohol == true)
            .GroupBy(a => a.Distrito ?? "Desconocido")
            .Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(x => x.Total)
            .ToList();
        return resultado;
    }

    public List<(int CodDistrito, int Total)> AccidentesPorCodDistrito()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.CodDistrito)
            .Select(g => (CodDistrito: g.Key, Total: g.Count()))
            .ToList();
        return resultado;
    }

    public List<(int Anio, int Total)> AccidentesPorAnio()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.Year)
            .Select(g => (Anio: g.Key, Total: g.Count()))
            .ToList();
        return resultado;
    }

    public List<(int Anio, int Mes, int Total)> EvolucionMensualPorAnio()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => new { a.Fecha.Year, a.Fecha.Month })
            .Select(g => (Anio: g.Key.Year, Mes: g.Key.Month, Total: g.Count()))
            .OrderBy(x => x.Anio)
            .ThenBy(x => x.Mes)
            .ToList();
        return resultado;
    }

    public List<(int Anio, string Distrito, int Total)> DistritoConMasAccidentesPorAnio()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.Year)
            .Select(grupoAnio =>
            {
                var distritoTop = grupoAnio
                    .GroupBy(a => a.Distrito ?? "Desconocido")
                    .OrderByDescending(g => g.Count())
                    .First();

                return (Anio: grupoAnio.Key, Distrito: distritoTop.Key, Total: distritoTop.Count());
            })
            .OrderBy(x => x.Anio)
            .ToList();
        return resultado;
    }

    public List<(int Anio, int Total)> TendenciaAlcoholPorAnio()
    {
        var resultado = accidentes
            .Where(a => a.PositivaAlcohol == true)
            .GroupBy(a => a.Fecha.Year)
            .Select(g => (Anio: g.Key, Total: g.Count()))
            .OrderBy(x => x.Anio)
            .ToList();
        return resultado;
    }

    public List<(int Anio, int FinDeSemana, int EntreSemana)> ComparativaFinDeSemanaEntreSemanaPorAnio()
    {
        var accidentesUnicos = accidentes.DistinctBy(a => a.NumExpediente).ToList();

        var resultado = accidentesUnicos
            .GroupBy(a => a.Fecha.Year)
            .Select(g =>
            {
                int finDeSemana = g.Count(a => a.Fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);
                int entreSemana = g.Count() - finDeSemana;
                return (Anio: g.Key, FinDeSemana: finDeSemana, EntreSemana: entreSemana);
            })
            .OrderBy(x => x.Anio)
            .ToList();
        return resultado;
    }

    public List<(int Anio, int Hora, int Total)> HoraPicoPorAnio()
    {
        var resultado = accidentes
            .DistinctBy(a => a.NumExpediente)
            .GroupBy(a => a.Fecha.Year)
            .Select(grupoAnio =>
            {
                var horaTop = grupoAnio
                    .GroupBy(a => a.Hora.Hour)
                    .OrderByDescending(g => g.Count())
                    .First();

                return (Anio: grupoAnio.Key, Hora: horaTop.Key, Total: horaTop.Count());
            })
            .OrderBy(x => x.Anio)
            .ToList();
        return resultado;
    }

    public List<(int Anio, string Lesividad, int Total)> LesionMasFrecuentePorAnio()
    {
        var resultado = accidentes
            .GroupBy(a => a.Fecha.Year)
            .Select(grupoAnio =>
            {
                var lesionTop = grupoAnio
                    .GroupBy(a => a.Lesividad ?? "Desconocido")
                    .OrderByDescending(g => g.Count())
                    .First();

                return (Anio: grupoAnio.Key, Lesividad: lesionTop.Key, Total: lesionTop.Count());
            })
            .OrderBy(x => x.Anio)
            .ToList();
        return resultado;
    }

    public List<(int Anio, int Total)> EvolucionPeatonesPorAnio()
    {
        var resultado = accidentes
            .Where(a => a.TipoPersona == "Peatón")
            .GroupBy(a => a.Fecha.Year)
            .Select(g => (Anio: g.Key, Total: g.Count()))
            .OrderBy(x => x.Anio)
            .ToList();
        return resultado;
    }
}