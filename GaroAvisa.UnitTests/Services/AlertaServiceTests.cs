using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Application.Services;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using Moq;

namespace GaroAvisa.UnitTests.Services
{
    public class AlertaServiceTests
    {
        private readonly Mock<IAlertaRepository> _mockAlertas = new();
        private readonly Mock<ILocalRepository> _mockLocais = new();
        private readonly Mock<IPreferenciaRepository> _mockPreferencias = new();
        private readonly Mock<IRegraAlertaRepository> _mockRegras = new();
        private readonly Mock<IPrevisaoSalvaRepository> _mockPrevisoes = new();
        private readonly Mock<IOpenMeteoClient> _mockOpenMeteo = new();
        private readonly AlertaService _service;

        private readonly Local _casa = new Local { Id = 1, UsuarioId = 7, Etiqueta = "Casa", Latitude = -23.5, Longitude = -46.5, ReceberAlertas = true };

        public AlertaServiceTests()
        {
            _mockLocais.Setup(r => r.ListarParaAlertasAsync()).ReturnsAsync(new List<Local> { _casa });
            _mockRegras.Setup(r => r.ListarAsync(true)).ReturnsAsync(new List<RegraAlerta>
            {
                new RegraAlerta { Tipo = "CHUVA_FORTE", Limite = 20, Gravidade = "ALTA", Titulo = "Chuva forte", Descricao = "Evite alagamentos.", Ativa = true }
            });
            _mockOpenMeteo.Setup(o => o.ObterPrevisaoAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), 2))
                .ReturnsAsync(new List<PrevisaoDiaDto>
                {
                    new PrevisaoDiaDto { Data = "2026-11-10", ChuvaMm = 32, CodigoTempo = 63 }
                });

            _service = new AlertaService(_mockAlertas.Object, _mockLocais.Object, _mockPreferencias.Object,
                _mockRegras.Object, _mockPrevisoes.Object, _mockOpenMeteo.Object);
        }

        [Fact]
        public async Task Verificar_ChuvaAcimaDoLimite_CriaAlerta()
        {
            // Arrange: usuário sem preferências salvas (usa o padrão: tudo ligado)
            _mockAlertas.Setup(r => r.ExisteAsync(1, It.IsAny<DateTime>(), "CHUVA_FORTE")).ReturnsAsync(false);

            // Act
            var resultado = await _service.VerificarAsync();

            // Assert
            Assert.Equal(1, resultado.LocaisVerificados);
            Assert.Equal(1, resultado.AlertasCriados);
            _mockAlertas.Verify(r => r.CriarAsync(It.Is<Alerta>(a => a.Tipo == "CHUVA_FORTE" && a.Gravidade == "ALTA" && a.ValorPrevisto == 32)), Times.Once);
        }

        [Fact]
        public async Task Verificar_AlertaJaExiste_NaoDuplica()
        {
            // Arrange
            _mockAlertas.Setup(r => r.ExisteAsync(1, It.IsAny<DateTime>(), "CHUVA_FORTE")).ReturnsAsync(true);

            // Act
            var resultado = await _service.VerificarAsync();

            // Assert
            Assert.Equal(0, resultado.AlertasCriados);
            _mockAlertas.Verify(r => r.CriarAsync(It.IsAny<Alerta>()), Times.Never);
        }

        [Fact]
        public async Task Verificar_UsuarioDesligouAlertasDoDia_NaoCriaAlerta()
        {
            // Arrange
            _mockPreferencias.Setup(r => r.BuscarPorUsuarioAsync(7))
                .ReturnsAsync(new Preferencia { UsuarioId = 7, AlertasDoDia = false });

            // Act
            var resultado = await _service.VerificarAsync();

            // Assert
            Assert.Equal(0, resultado.AlertasCriados);
            _mockAlertas.Verify(r => r.CriarAsync(It.IsAny<Alerta>()), Times.Never);
        }

        [Fact]
        public async Task Verificar_OpenMeteoForaDoAr_ContaFalhaESegue()
        {
            // Arrange
            _mockOpenMeteo.Setup(o => o.ObterPrevisaoAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), 2))
                .ThrowsAsync(new HttpRequestException("fora do ar"));

            // Act
            var resultado = await _service.VerificarAsync();

            // Assert
            Assert.Equal(1, resultado.LocaisComFalha);
            Assert.Equal(0, resultado.LocaisVerificados);
        }
    }
}
