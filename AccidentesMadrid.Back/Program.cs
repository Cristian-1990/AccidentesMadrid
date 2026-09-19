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