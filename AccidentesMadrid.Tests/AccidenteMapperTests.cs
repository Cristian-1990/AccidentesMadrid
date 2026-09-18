using AccidentesMadrid.Back.Mappers;
using FluentAssertions;

namespace AccidentesMadrid.Tests.Mappers;
/// <summary>
/// Verifica el comportamiento del mapper con distintos valores en distintos casos.
/// Evalua los valores posibles de un caso correcto, PositivoAlcohol y postivoDroga.
/// También los dos casos formato de fecha incorrecto y CodDistritoNoNumerico.
/// Con patrón AAA
/// </summary>
[TestFixture]
public class AccidenteMapperTests
{
    
    
    [Test]
    public void ToAccidente_FilaCompleta_MapeaTodosLosCampos()
    {
        //Arrange
        var fila = new AccidenteCsv
        {
            NumExpediente = "2025S000056",
            Fecha = "01/01/2025",
            Hora = "0:49:00",
            Localizacion = "CALL. LOPEZ DE HOYOS / CALL. ROS DE OLANO",
            Numero = "140",
            CodDistrito = "5",
            Distrito = "CHAMARTÍN",
            TipoAccidente = "Colisión fronto-lateral",
            EstadoMeteorologico = "Despejado",
            TipoVehiculo = "Ciclomotor",
            TipoPersona = "Conductor",
            RangoEdad = "De 30 a 34 años",
            Sexo = "Hombre",
            CodLesividad = "7",
            Lesividad = "Asistencia sanitaria sólo en el lugar del accidente",
            CoordenadaXUtm = "442966",
            CoordenadaYUtm = "4477385",
            PositivaAlcohol = "N",
            PositivaDroga = "",
        };
        //Act
        var resultado = fila.ToAccidente();
        //Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.NumExpediente.Should().Be("2025S000056");
        resultado.Value.CodDistrito.Should().Be(5);
        resultado.Value.CodLesividad.Should().Be(7);
        resultado.Value.CoordenadaXUtm.Should().Be(442966);
        resultado.Value.PositivaAlcohol.Should().BeFalse();
        resultado.Value.PositivaDroga.Should().BeFalse();
    }
    
    
    /// <summary>
    /// No existe positivoDroga = 2 = false.
    /// Lo que no sea 1 será false
    /// </summary>
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
    public void ToAccidente_PositivaDroga1_DevuelveTrue()
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
    
    /// <summary>
    /// "2025-01-01" es una fecha correcta pero hemos fijado el formato dd/mm/aaaa.
    /// Debe fallar aunque la fecha en si tenga sentido y se correctae.
    /// </summary>
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