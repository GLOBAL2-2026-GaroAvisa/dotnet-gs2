using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Services;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using Moq;

namespace GaroAvisa.UnitTests.Services
{
    public class LocalServiceTests
    {
        private readonly Mock<ILocalRepository> _mockRepository;
        private readonly LocalService _service;

        public LocalServiceTests()
        {
            _mockRepository = new Mock<ILocalRepository>();
            _service = new LocalService(_mockRepository.Object);
        }

        [Fact]
        public async Task Criar_ApelidoRepetido_LancaInvalidOperationException()
        {
            // Arrange
            _mockRepository.Setup(r => r.ApelidoExisteAsync("Casa", 1, null)).ReturnsAsync(true);
            var dto = new CriarLocalDto { Apelido = "Casa", Nome = "Tatuapé", Latitude = -23.5, Longitude = -46.5 };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto, 1));
        }

        [Fact]
        public async Task Criar_LocalPrincipal_TiraPrincipalDosOutrosLocais()
        {
            // Arrange
            _mockRepository.Setup(r => r.ApelidoExisteAsync("Casa", 1, null)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.CriarAsync(It.IsAny<Local>())).ReturnsAsync((Local l) => l);
            var dto = new CriarLocalDto { Apelido = "Casa", Nome = "Tatuapé", Latitude = -23.5, Longitude = -46.5, Principal = true };

            // Act
            var criado = await _service.CriarAsync(dto, 1);

            // Assert
            Assert.True(criado.Principal);
            _mockRepository.Verify(r => r.RemoverPrincipalAsync(1, null), Times.Once);
        }

        [Fact]
        public async Task Buscar_LocalDeOutroUsuario_RetornaNull()
        {
            // Arrange: o repositório só encontra o local quando o dono é o usuário 1
            _mockRepository.Setup(r => r.BuscarPorIdAsync(10, 2)).ReturnsAsync((Local?)null);

            // Act
            var local = await _service.BuscarPorIdAsync(10, 2);

            // Assert
            Assert.Null(local);
        }
    }
}
