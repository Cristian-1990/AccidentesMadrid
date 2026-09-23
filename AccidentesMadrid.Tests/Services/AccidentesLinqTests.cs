using AccidentesMadrid.Back.Repositories;
using AccidentesMadrid.Back.Services;
using FluentAssertions;

namespace AccidentesMadrid.Tests.Services;

[TestFixture]
public class AccidentesLinqTests
{
    [Test]
    public async Task TotalAccidentes_MismoExpedienteVariasFilas_CuentaUnaVez()
    {
        // Arrange: prueba_dedup.csv tiene 3 filas pero solo 2 num_expediente distintos
        var repo = new AccidentesRepository();
        var resultado = await repo.LeerAccidentesAsync(["TestData/prueba_dedup.csv"]);
        var analyzer = new AccidentesLinq(resultado.Value.Accidentes);

        // Act
        var total = analyzer.TotalAccidentes();

        // Assert
        total.Should().Be(2);
    }

    [Test]
    public async Task AccidentesPorDistritoTop5_MismoExpedienteVariasFilas_NoDuplicaElAccidente()
    {
        // Arrange
        var repo = new AccidentesRepository();
        var resultado = await repo.LeerAccidentesAsync(["TestData/prueba_dedup.csv"]);
        var analyzer = new AccidentesLinq(resultado.Value.Accidentes);

        // Act
        var porDistrito = analyzer.AccidentesPorDistritoTop5();

        // Assert: CENTRO tiene 2 filas (2 personas) pero es 1 solo accidente
        porDistrito.Should().ContainSingle(x => x.Distrito == "CENTRO" && x.Total == 1);
    }
}