using System.Diagnostics;
using AccidentesMadrid.Back.Repositories;
using Microsoft.Data.Analysis;
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





var tiposColumna = Enumerable.Repeat(typeof(string), 19).ToArray();


string[] rutasCsv = ["data/Accidentes-2024.csv", "data/Accidentes-2025.csv", "data/Accidentes-2026.csv"];

using var ms = new MemoryStream();
using (var writer = new StreamWriter(ms, leaveOpen: true))
{
    for (int i = 0; i < rutasCsv.Length; i++)
    {
        var lineas = File.ReadLines(rutasCsv[i]);
        foreach (var linea in i == 0 ? lineas : lineas.Skip(1)) // salta la cabecera repetida en 2º y 3er fichero
            writer.WriteLine(linea);
    }
}
ms.Position = 0;

var df = DataFrame.LoadCsv(ms, separator: ';', dataTypes: tiposColumna);
var dataFrame = new AccidentesDataFrame(df);
//=========DATAFRAME======================
//=======CONSULTAS DATAFRAME============
Medir("1. Total accidentes (DF)", () => dataFrame.TotalAccidentes());
Medir("2. Accidentes por distrito (top 5) (DF)", () => dataFrame.AccidentesPorDistritoTop5());
Medir("3. Accidentes por tipo (DF)", () => dataFrame.AccidentesPorTipo());
Medir("4. Accidentes por estado meteorológico (DF)", () => dataFrame.AccidentesPorEstadoMeteorologico());
Medir("5. Accidentes por sexo (DF)", () => dataFrame.AccidentesPorSexo());
Medir("6. Accidentes por rango de edad (DF)", () => dataFrame.AccidentesPorRangoEdad());
Medir("7. Positivos en alcohol (DF)", () => dataFrame.PositivosEnAlcohol());
Medir("8. Positivos en drogas (DF)", () => dataFrame.PositivosEnDrogas());
Medir("9. Accidentes por día de la semana (DF)", () => dataFrame.AccidentesPorDiaSemana());
Medir("10. Accidentes por mes (DF)", () => dataFrame.AccidentesPorMes());
Medir("11. Hora con más accidentes (DF)", () => dataFrame.HoraConMasAccidentes());
Medir("12. Lesiones más frecuentes (DF)", () => dataFrame.LesionesMasFrecuentes());
Medir("13. Tipo de vehículo más implicado (DF)", () => dataFrame.TipoVehiculoMasImplicado());
Medir("14. Accidentes con peatones (DF)", () => dataFrame.AccidentesConPeatones());
Medir("15. Proporción hombre/mujer (DF)", () => dataFrame.ProporcionHombreMujer());
Medir("16. Distritos con más peatones (DF)", () => dataFrame.DistritosConMasPeatones());
Medir("17. Fin de semana vs entre semana (DF)", () => dataFrame.FinDeSemanaVsEntreSemana());
Medir("18. Media de accidentes por día (DF)", () => dataFrame.MediaAccidentesPorDia());
Medir("19. Accidentes con alcohol + droga (DF)", () => dataFrame.AccidentesConAlcoholYDroga());
Medir("20. Rangos de edad más vulnerables (peatones) (DF)", () => dataFrame.RangosEdadMasVulnerablesPeatones());
Medir("21. Distritos con más positivos en alcohol (DF)", () => dataFrame.DistritosConMasPositivosAlcohol());
Medir("22. Accidentes por código de distrito (DF)", () => dataFrame.AccidentesPorCodDistrito());
Medir("23. Accidentes por año (DF)", () => dataFrame.AccidentesPorAnio());
Medir("24. Evolución mensual por año (DF)", () => dataFrame.EvolucionMensualPorAnio());
Medir("25. Distrito con más accidentes por año (DF)", () => dataFrame.DistritoConMasAccidentesPorAnio());
Medir("26. Tendencia de alcohol por año (DF)", () => dataFrame.TendenciaAlcoholPorAnio());
Medir("27. Comparativa fin de semana vs entre semana por año (DF)", () => dataFrame.ComparativaFinDeSemanaEntreSemanaPorAnio());
Medir("28. Hora pico por año (DF)", () => dataFrame.HoraPicoPorAnio());
Medir("29. Lesión más frecuente por año (DF)", () => dataFrame.LesionMasFrecuentePorAnio());
Medir("30. Evolución de peatones por año (DF)", () => dataFrame.EvolucionPeatonesPorAnio());
//=========MEDIDOR DE CONSULTAS===============
static void Medir<T>(string nombre, Func<T> consulta)
{
    var sw = Stopwatch.StartNew();
    var resultado = consulta();
    sw.Stop();
    Console.WriteLine($"{nombre}: {sw.ElapsedMilliseconds} ms");
}