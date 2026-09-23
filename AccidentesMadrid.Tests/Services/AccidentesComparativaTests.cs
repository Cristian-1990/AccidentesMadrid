using AccidentesMadrid.Back.Repositories;
using AccidentesMadrid.Back.Services;
using FluentAssertions;
using Microsoft.Data.Analysis;

namespace AccidentesMadrid.Tests.Services;

[TestFixture]
public class AccidentesComparativaTests
{
    private static AccidentesDataFrame CargarDataFrame(string ruta)
    {
        var tiposColumna = Enumerable.Repeat(typeof(string), 19).ToArray();
        var df = DataFrame.LoadCsv(ruta, separator: ';', dataTypes: tiposColumna);
        return new AccidentesDataFrame(df);
    }

    [Test]
    public async Task TotalAccidentes_MismoDataset_LinqYDataFrameCoinciden()
    {
        // Arrange
        var repo = new AccidentesRepository();
        var resultado = await repo.LeerAccidentesAsync(["TestData/prueba_dedup.csv"]);
        var linq = new AccidentesLinq(resultado.Value.Accidentes);
        var dataFrame = CargarDataFrame("TestData/prueba_dedup.csv");

        // Act
        var totalLinq = linq.TotalAccidentes();
        var totalDataFrame = dataFrame.TotalAccidentes();

        // Assert
        totalDataFrame.Should().Be(totalLinq);
    }

    [Test]
    public async Task AccidentesPorDistritoTop5_MismoDataset_LinqYDataFrameCoinciden()
    {
        // Arrange
        var repo = new AccidentesRepository();
        var resultado = await repo.LeerAccidentesAsync(["TestData/prueba_dedup.csv"]);
        var linq = new AccidentesLinq(resultado.Value.Accidentes);
        var dataFrame = CargarDataFrame("TestData/prueba_dedup.csv");

        // Act
        var porDistritoLinq = linq.AccidentesPorDistritoTop5();
        var porDistritoDataFrame = dataFrame.AccidentesPorDistritoTop5();

        // Assert: mismo contenido en ambas listas, sin importar el orden
        porDistritoDataFrame.Should().BeEquivalentTo(porDistritoLinq);
    }
}