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
/// para poder construir la coleccion de accidentes sobre lka que vamos a trabajr las consultas
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
    /// Lee los archivos de principio a fin creando dos listas, una de Accidentes correctos y otra con los descartes
    /// </summary>
    /// <param name="rutas">Ruta en la que se encuntran los archivos</param>
    /// <returns>Devuelve las dos listas de Accidentes o DomainError</returns>
    public Result<(List<Accidente> Accidentes, List<DomainError> Descartados), DomainError> LeerAccidentes(string[] rutas)
    {
        var comprobacion = ComprobarFicheros(rutas);
        if (comprobacion.IsFailure)
            return Result.Failure<(List<Accidente>, List<DomainError>), DomainError>(comprobacion.Error);

        var accidentes = new List<Accidente>();
        var descartados = new List<DomainError>();

        //Cambiamos el separador por defecto "," por ";"
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        //Una vez verificada las rutas existentes las recorre y va añadiendo filas.
        //Tanto si son accidentes correctos como si son descartes.
        foreach (var ruta in rutas)
        {
            using var reader = new StreamReader(ruta);
            using var csv = new CsvReader(reader, config);
            var filas = csv.GetRecords<AccidenteCsv>();
            //Para después recorrer el IEnumerable de accidentes y añadirla en descarte o accidente
            foreach (var fila in filas)
            {
                var resultado = fila.ToAccidente();
                if (resultado.IsSuccess)
                    accidentes.Add(resultado.Value);
                else
                    descartados.Add(resultado.Error);
            }
        }

        return Result.Success<(List<Accidente>, List<DomainError>), DomainError>((accidentes, descartados));
    }
}