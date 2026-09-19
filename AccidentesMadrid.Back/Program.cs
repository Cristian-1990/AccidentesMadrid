using System.Diagnostics;
using AccidentesMadrid.Back.Repositories;

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

//=========LECTURA SECUENCIAL==================
var stopwatchParalelo = Stopwatch.StartNew();
var resultado = await repo.LeerAccidentesAsync(rutas);
stopwatchParalelo.Stop();
Console.WriteLine($"Paralelo: {stopwatchParalelo.ElapsedMilliseconds} ms, {resultado.Value.Accidentes.Count} accidentes");