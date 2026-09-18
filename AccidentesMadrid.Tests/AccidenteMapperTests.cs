using AccidentesMadrid.Back.Mappers;
using FluentAssertions;

namespace AccidentesMadrid.Tests.Mappers;

[TestFixture]
public class AccidenteMapperTests
{
    [Test]
    public void ToAccidente_PositivaDrogaVacio_DevuelveFalse()
    {
        // Arrange
        var fila = new AccidenteCsv
        {
            NumExpediente = "2025S000057",
            Fecha = "01/01/2025",
            Hora = "2:20:00",
            CodDistrito = "9",
            PositivaDroga = "",
        };

        // Act
        var resultado = fila.ToAccidente();

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.PositivaDroga.Should().BeFalse();
    }
    [Test]
public void ToAccidente_PositivaAlcoholS_DevuelveTrue()
{
    var fila = new AccidenteCsv
    {
        NumExpediente = "1", Fecha = "01/01/2025", Hora = "10:00:00",
        CodDistrito = "5", PositivaAlcohol = "S",
    };

    var resultado = fila.ToAccidente();

    resultado.IsSuccess.Should().BeTrue();
    resultado.Value.PositivaAlcohol.Should().BeTrue();
}

[Test]
public void ToAccidente_PositivaAlcoholN_DevuelveFalse()
{
    var fila = new AccidenteCsv
    {
        NumExpediente = "1", Fecha = "01/01/2025", Hora = "10:00:00",
        CodDistrito = "5", PositivaAlcohol = "N",
    };

    var resultado = fila.ToAccidente();

    resultado.IsSuccess.Should().BeTrue();
    resultado.Value.PositivaAlcohol.Should().BeFalse();
}

[Test]
public void ToAccidente_PositivaAlcoholVacio_DevuelveNull()
{
    var fila = new AccidenteCsv
    {
        NumExpediente = "1", Fecha = "01/01/2025", Hora = "10:00:00",
        CodDistrito = "5", PositivaAlcohol = "",
    };

    var resultado = fila.ToAccidente();

    resultado.IsSuccess.Should().BeTrue();
    resultado.Value.PositivaAlcohol.Should().BeNull();
}

[Test]
public void ToAccidente_PositivaDrogaUno_DevuelveTrue()
{
    var fila = new AccidenteCsv
    {
        NumExpediente = "1", Fecha = "01/01/2025", Hora = "10:00:00",
        CodDistrito = "5", PositivaDroga = "1",
    };

    var resultado = fila.ToAccidente();

    resultado.IsSuccess.Should().BeTrue();
    resultado.Value.PositivaDroga.Should().BeTrue();
}

[Test]
public void ToAccidente_CodDistritoNoNumerico_DevuelveFailure()
{
    var fila = new AccidenteCsv
    {
        NumExpediente = "1", Fecha = "01/01/2025", Hora = "10:00:00",
        CodDistrito = "ABC",
    };

    var resultado = fila.ToAccidente();

    resultado.IsFailure.Should().BeTrue();
}

[Test]
public void ToAccidente_FechaFormatoIncorrecto_DevuelveFailure()
{
    var fila = new AccidenteCsv
    {
        NumExpediente = "1", Fecha = "2025-01-01", Hora = "10:00:00",
        CodDistrito = "5",
    };

    var resultado = fila.ToAccidente();

    resultado.IsFailure.Should().BeTrue();
}
}