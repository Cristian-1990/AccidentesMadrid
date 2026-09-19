using AccidentesMadrid.Back.Repositories;
using FluentAssertions;

namespace AccidentesMadrid.Tests.Repositories;

[TestFixture]
public class AccidentesRepositoryTests
{
    [Test]
    public async Task LeerAccidentesAsync_FicheroNoExiste_DevuelveFailure()
    {
        // Arrange
        var repo = new AccidentesRepository();

        // Act
        var resultado = await repo.LeerAccidentesAsync(["ruta_que_no_existe.csv"]);

        // Assert
        resultado.IsFailure.Should().BeTrue();
    }

    [Test]
    public async Task LeerAccidentesAsync_FicheroValido_DevuelveAccidentesYDescartes()
    {
        // Arrange
        var repo = new AccidentesRepository();

        // Act
        var resultado = await repo.LeerAccidentesAsync(["TestData/prueba.csv"]);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Accidentes.Should().HaveCount(2);
        resultado.Value.Descartados.Should().HaveCount(1);
    }
}