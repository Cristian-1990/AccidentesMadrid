using System.Diagnostics;
using AccidentesMadrid.Back.Repositories;
using AccidentesMadrid.Back.Services;

var repo = new AccidentesRepository();

string[] rutas = [
    "data/Accidentes-2024.csv",
    "data/Accidentes-2025.csv",
    "data/Accidentes-2026.csv"
];

//=========LECTURA SECUENCIAL==================
var stopwatchSecuencial = Stopwatch.StartNew();
var totalAccidentesSecuencial = 0;
foreach (var ruta in rutas)
{
    var resultadoFichero = await repo.LeerAccidentesAsync([ruta]);
    totalAccidentesSecuencial += resultadoFichero.Value.Accidentes.Count;
}
stopwatchSecuencial.Stop();
Console.WriteLine($"Secuencial: {stopwatchSecuencial.ElapsedMilliseconds} ms, {totalAccidentesSecuencial} accidentes");

//=========LECTURA PARALELO==================
var stopwatchParalelo = Stopwatch.StartNew();
var resultado = await repo.LeerAccidentesAsync(rutas);
stopwatchParalelo.Stop();
Console.WriteLine($"Paralelo: {stopwatchParalelo.ElapsedMilliseconds} ms, {resultado.Value.Accidentes.Count} accidentes");

//=======CONSULTAS LOINQ============
var analyzer = new AccidentesLinq(resultado.Value.Accidentes);

Console.WriteLine($"Total accidentes: {analyzer.TotalAccidentes()}");
Console.WriteLine($"Hora con más accidentes: {analyzer.HoraConMasAccidentes()}");
Console.WriteLine($"Tipo de vehículo más implicado: {analyzer.TipoVehiculoMasImplicado()}");

foreach (var (anio, distrito, total) in analyzer.DistritoConMasAccidentesPorAnio())
    Console.WriteLine($"{anio}: {distrito} ({total} accidentes)");



Medir("1. Total accidentes", () => analyzer.TotalAccidentes());
Medir("2. Accidentes por distrito (top 5)", () => analyzer.AccidentesPorDistritoTop5());
Medir("3. Accidentes por tipo", () => analyzer.AccidentesPorTipo());
Medir("4. Accidentes por estado meteorológico", () => analyzer.AccidentesPorEstadoMeteorologico());
Medir("5. Accidentes por sexo", () => analyzer.AccidentesPorSexo());
Medir("6. Accidentes por rango de edad", () => analyzer.AccidentesPorRangoEdad());
Medir("7. Positivos en alcohol", () => analyzer.PositivosEnAlcohol());
Medir("8. Positivos en drogas", () => analyzer.PositivosEnDrogas());
Medir("9. Accidentes por día de la semana", () => analyzer.AccidentesPorDiaSemana());
Medir("10. Accidentes por mes", () => analyzer.AccidentesPorMes());
Medir("11. Hora con más accidentes", () => analyzer.HoraConMasAccidentes());
Medir("12. Lesiones más frecuentes", () => analyzer.LesionesMasFrecuentes());
Medir("13. Tipo de vehículo más implicado", () => analyzer.TipoVehiculoMasImplicado());
Medir("14. Accidentes con peatones", () => analyzer.AccidentesConPeatones());
Medir("15. Proporción hombre/mujer", () => analyzer.ProporcionHombreMujer());
Medir("16. Distritos con más peatones", () => analyzer.DistritosConMasPeatones());
Medir("17. Fin de semana vs entre semana", () => analyzer.FinDeSemanaVsEntreSemana());
Medir("18. Media de accidentes por día", () => analyzer.MediaAccidentesPorDia());
Medir("19. Accidentes con alcohol + droga", () => analyzer.AccidentesConAlcoholYDroga());
Medir("20. Rangos de edad más vulnerables (peatones)", () => analyzer.RangosEdadMasVulnerablesPeatones());
Medir("21. Distritos con más positivos en alcohol", () => analyzer.DistritosConMasPositivosAlcohol());
Medir("22. Accidentes por código de distrito", () => analyzer.AccidentesPorCodDistrito());
Medir("23. Accidentes por año", () => analyzer.AccidentesPorAnio());
Medir("24. Evolución mensual por año", () => analyzer.EvolucionMensualPorAnio());
Medir("25. Distrito con más accidentes por año", () => analyzer.DistritoConMasAccidentesPorAnio());
Medir("26. Tendencia de alcohol por año", () => analyzer.TendenciaAlcoholPorAnio());
Medir("27. Comparativa fin de semana vs entre semana por año", () => analyzer.ComparativaFinDeSemanaEntreSemanaPorAnio());
Medir("28. Hora pico por año", () => analyzer.HoraPicoPorAnio());
Medir("29. Lesión más frecuente por año", () => analyzer.LesionMasFrecuentePorAnio());
Medir("29b. Lesión más frecuente por año (PLINQ)", () => analyzer.LesionMasFrecuentePorAnioParalelo());
Medir("30. Evolución de peatones por año", () => analyzer.EvolucionPeatonesPorAnio());


//=========MEDIDOR DE CONSULTAS===============
static void Medir<T>(string nombre, Func<T> consulta)
{
    var sw = Stopwatch.StartNew();
    var resultado = consulta();
    sw.Stop();
    Console.WriteLine($"{nombre}: {sw.ElapsedMilliseconds} ms");
}