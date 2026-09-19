using AccidentesMadrid.Back.Errors;
using AccidentesMadrid.Back.Errors.Common;
using AccidentesMadrid.Back.Models;
using AccidentesMadrid.Back.Mappers;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using CSharpFunctionalExtensions;

namespace AccidentesMadrid.Back.Repositories;
/// <summary>
/// Gestiona los ficheros del repo y comprueba que existen,
/// para poder construir la coleccion de accidentes sobre la que vamos a trabajar las consultas
/// </summary>
public class AccidentesRepository
{
    /// <summary>
    /// Recibe un string de rutas y comprueba que existen antes de intentar leerlos.
    /// </summary>
    /// <param name="rutas">rutas de los ficheros a comprobar</param>
    /// <returns>Devuelve un string de rutas correctas o Domain Error</returns>
    private Result<string[], DomainError> ComprobarFicheros(string[] rutas)
    {
        var faltantes = rutas.Where(r => !File.Exists(r)).ToList();

        if (faltantes.Any())
            return Result.Failure<string[], DomainError>(AccidenteErrors.FicheroNoEncontrado(faltantes));

        return Result.Success<string[], DomainError>(rutas);
    }

    /// <summary>
    /// Lee un unico fichero CSV y devuelve sus propias listas de accidentes y descartes.
    /// No comparte estado con otras llamadas, para poder ejecutarse en paralelo sin conflictos.
    /// </summary>
    /// <param name="ruta">Ruta del fichero a leer</param>
    /// <param name="config">Configuracion de CsvHelper (separador, cultura...)</param>
    /// <returns>Tupla con los accidentes correctos y los descartes de ese fichero</returns>
    private (List<Accidente> Accidentes, List<DomainError> Descartados) LeerFichero(string ruta, CsvConfiguration config)
    {
        var accidentes = new List<Accidente>();
        var descartados = new List<DomainError>();

        using var reader = new StreamReader(ruta);
        using var csv = new CsvReader(reader, config);
        var filas = csv.GetRecords<AccidenteCsv>();

        foreach (var fila in filas)
        {
            var resultado = fila.ToAccidente();
            if (resultado.IsSuccess)
                accidentes.Add(resultado.Value);
            else
                descartados.Add(resultado.Error);
        }

        return (accidentes, descartados);
    }

    /// <summary>
    /// Lee los ficheros en paralelo (uno por hilo con Task.Run) y combina sus resultados
    /// una vez que todos han terminado.
    /// </summary>
    /// <param name="rutas">Rutas de los ficheros a leer</param>
    /// <returns>Devuelve las dos listas combinadas de Accidentes o DomainError</returns>
    public async Task<Result<(List<Accidente> Accidentes, List<DomainError> Descartados), DomainError>> LeerAccidentesAsync(string[] rutas)
    {
        var comprobacion = ComprobarFicheros(rutas);
        if (comprobacion.IsFailure)
            return Result.Failure<(List<Accidente>, List<DomainError>), DomainError>(comprobacion.Error);

        //Cambiamos el separador por defecto "," por ";"
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };

        var tareas = rutas.Select(ruta => Task.Run(() => LeerFichero(ruta, config))).ToArray();
        var resultados = await Task.WhenAll(tareas);

        var accidentes = new List<Accidente>();
        var descartados = new List<DomainError>();

        foreach (var resultado in resultados)
        {
            accidentes.AddRange(resultado.Accidentes);
            descartados.AddRange(resultado.Descartados);
        }

        return Result.Success<(List<Accidente>, List<DomainError>), DomainError>((accidentes, descartados));
    }
}