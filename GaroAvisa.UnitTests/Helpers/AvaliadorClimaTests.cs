using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Helpers;
using GaroAvisa.Domain.Entities;

namespace GaroAvisa.UnitTests.Helpers
{
    public class AvaliadorClimaTests
    {
        private static List<RegraAlerta> RegrasDeChuva()
        {
            return new List<RegraAlerta>
            {
                new RegraAlerta { Id = 1, Tipo = "CHUVA_FORTE", Limite = 15, Gravidade = "MEDIA", Ativa = true },
                new RegraAlerta { Id = 2, Tipo = "CHUVA_FORTE", Limite = 30, Gravidade = "ALTA", Ativa = true }
            };
        }

        [Fact]
        public void EscolherRegra_ChuvaAcimaDasDuasRegras_EscolheAMaisGrave()
        {
            // Arrange
            var dia = new PrevisaoDiaDto { ChuvaMm = 32 };

            // Act
            var regra = AvaliadorClima.EscolherRegra(RegrasDeChuva(), "CHUVA_FORTE", dia);

            // Assert
            Assert.NotNull(regra);
            Assert.Equal("ALTA", regra.Gravidade);
        }

        [Fact]
        public void EscolherRegra_ChuvaAbaixoDoLimite_RetornaNull()
        {
            // Arrange
            var dia = new PrevisaoDiaDto { ChuvaMm = 5 };

            // Act
            var regra = AvaliadorClima.EscolherRegra(RegrasDeChuva(), "CHUVA_FORTE", dia);

            // Assert
            Assert.Null(regra);
        }

        [Fact]
        public void EscolherRegra_RegraDesativada_NaoEhUsada()
        {
            // Arrange
            var regras = new List<RegraAlerta>
            {
                new RegraAlerta { Tipo = "CALOR", Limite = 30, Gravidade = "MEDIA", Ativa = false }
            };
            var dia = new PrevisaoDiaDto { TemperaturaMaxima = 38 };

            // Act
            var regra = AvaliadorClima.EscolherRegra(regras, "CALOR", dia);

            // Assert
            Assert.Null(regra);
        }

        [Theory]
        [InlineData(5, true)]
        [InlineData(8, true)]
        [InlineData(12, false)]
        public void Ultrapassou_Frio_DisparaQuandoFicaAbaixoDoLimite(double temperaturaMinima, bool esperado)
        {
            // Act
            var resultado = AvaliadorClima.Ultrapassou("FRIO", temperaturaMinima, 8);

            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void DescreverMudanca_ChuvaAumentou10mm_DescreveAMudanca()
        {
            // Arrange
            var antes = new PrevisaoSalva { ChuvaMm = 2, CodigoTempo = 3, TemperaturaMaxima = 25 };
            var agora = new PrevisaoDiaDto { ChuvaMm = 25, CodigoTempo = 63, TemperaturaMaxima = 24 };

            // Act
            var mudanca = AvaliadorClima.DescreverMudanca(antes, agora);

            // Assert
            Assert.NotNull(mudanca);
            Assert.Contains("chuva de 2 mm para 25 mm", mudanca);
            Assert.Contains("Chuva moderada", mudanca);
        }

        [Fact]
        public void DescreverMudanca_MudancaPequena_RetornaNull()
        {
            // Arrange
            var antes = new PrevisaoSalva { ChuvaMm = 2, CodigoTempo = 3, TemperaturaMaxima = 25 };
            var agora = new PrevisaoDiaDto { ChuvaMm = 4, CodigoTempo = 2, TemperaturaMaxima = 27 };

            // Act
            var mudanca = AvaliadorClima.DescreverMudanca(antes, agora);

            // Assert
            Assert.Null(mudanca);
        }

        [Fact]
        public void CodigoTempo_Descrever_TraduzCodigoDaOpenMeteo()
        {
            Assert.Equal("Trovoada", CodigoTempo.Descrever(95));
            Assert.Equal("Condição desconhecida", CodigoTempo.Descrever(12345));
        }
    }
}
