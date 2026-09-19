using System.Diagnostics;
using AccidentesMadrid.Back.Repositories;

var repo = new AccidentesRepository();
var stopwatch = Stopwatch.StartNew();

var resultado = repo.LeerAccidentes(["data/Accidentes-2025.csv"]);

stopwatch.Stop();

Console.WriteLine($"Tiempo: {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"Accidentes: {resultado.Value.Accidentes.Count}");
Console.WriteLine($"Descartados: {resultado.Value.Descartados.Count}");