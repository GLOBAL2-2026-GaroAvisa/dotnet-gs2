using System.Security.Claims;
using GaroAvisa.Api.Controllers;
using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GaroAvisa.UnitTests.Controllers
{
    public class LocaisControllerTests
    {
        private readonly Mock<ILocalService> _mockService;
        private readonly LocaisController _controller;

        public LocaisControllerTests()
        {
            _mockService = new Mock<ILocalService>();
            _controller = new LocaisController(_mockService.Object);

            // Simula um usuário logado com ID 1 (o que viria dentro do token JWT)
            var usuario = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "Teste"));
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = usuario } };
        }

        [Fact]
        public async Task Buscar_LocalExiste_RetornaOk()
        {
            // Arrange
            var local = new LocalRespostaDto { Id = 5, Etiqueta = "Casa" };
            _mockService.Setup(s => s.BuscarPorIdAsync(5, 1)).ReturnsAsync(local);

            // Act
            var resultado = await _controller.Buscar(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var localRetornado = Assert.IsType<LocalRespostaDto>(okResult.Value);
            Assert.Equal("Casa", localRetornado.Etiqueta);
        }

        [Fact]
        public async Task Buscar_LocalNaoExiste_RetornaNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.BuscarPorIdAsync(99, 1)).ReturnsAsync((LocalRespostaDto?)null);

            // Act
            var resultado = await _controller.Buscar(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado);
        }

        [Fact]
        public async Task Criar_EtiquetaRepetida_RetornaConflict()
        {
            // Arrange
            _mockService.Setup(s => s.CriarAsync(It.IsAny<CriarLocalDto>(), 1))
                .ThrowsAsync(new InvalidOperationException("Você já tem um local com essa etiqueta."));

            // Act
            var resultado = await _controller.Criar(new CriarLocalDto { Etiqueta = "Casa", Nome = "Tatuapé" }, null);

            // Assert
            Assert.IsType<ConflictObjectResult>(resultado);
        }
    }
}
