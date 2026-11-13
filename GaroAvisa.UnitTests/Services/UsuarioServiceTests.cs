using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Application.Services;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using Moq;

namespace GaroAvisa.UnitTests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockRepository;
        private readonly Mock<ITokenService> _mockToken;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _mockRepository = new Mock<IUsuarioRepository>();
            _mockToken = new Mock<ITokenService>();
            _mockToken.Setup(t => t.GenerateToken(It.IsAny<Usuario>())).Returns("token-de-teste");

            _service = new UsuarioService(_mockRepository.Object, _mockToken.Object);
        }

        [Fact]
        public async Task Cadastrar_DadosValidos_SalvaHashENaoASenha()
        {
            // Arrange
            Usuario? usuarioSalvo = null;
            _mockRepository.Setup(r => r.EmailExisteAsync("ana@teste.com")).ReturnsAsync(false);
            _mockRepository.Setup(r => r.CriarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioSalvo = u)
                .ReturnsAsync((Usuario u) => u);

            var dto = new CadastroUsuarioDto { Nome = "Ana", Email = " Ana@Teste.com ", Senha = "123456" };

            // Act
            var resposta = await _service.CadastrarAsync(dto);

            // Assert
            Assert.Equal("token-de-teste", resposta.Token);
            Assert.NotNull(usuarioSalvo);
            Assert.Equal("ana@teste.com", usuarioSalvo.Email);
            Assert.Equal("USUARIO", usuarioSalvo.Tipo);
            Assert.NotEqual("123456", usuarioSalvo.SenhaHash);
            Assert.False(string.IsNullOrWhiteSpace(usuarioSalvo.SenhaHash));
        }

        [Fact]
        public async Task Cadastrar_EmailJaExiste_LancaInvalidOperationException()
        {
            // Arrange
            _mockRepository.Setup(r => r.EmailExisteAsync("ana@teste.com")).ReturnsAsync(true);
            var dto = new CadastroUsuarioDto { Nome = "Ana", Email = "ana@teste.com", Senha = "123456" };

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CadastrarAsync(dto));
            _mockRepository.Verify(r => r.CriarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Login_SenhaErrada_RetornaNull()
        {
            // Arrange: cadastra para gerar um hash de verdade
            Usuario? usuarioSalvo = null;
            _mockRepository.Setup(r => r.CriarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioSalvo = u)
                .ReturnsAsync((Usuario u) => u);
            await _service.CadastrarAsync(new CadastroUsuarioDto { Nome = "Ana", Email = "ana@teste.com", Senha = "123456" });
            _mockRepository.Setup(r => r.BuscarPorEmailAsync("ana@teste.com")).ReturnsAsync(usuarioSalvo);

            // Act
            var resposta = await _service.LoginAsync(new LoginDto { Email = "ana@teste.com", Senha = "senha-errada" });

            // Assert
            Assert.Null(resposta);
        }

        [Fact]
        public async Task Login_SenhaCerta_RetornaToken()
        {
            // Arrange
            Usuario? usuarioSalvo = null;
            _mockRepository.Setup(r => r.CriarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioSalvo = u)
                .ReturnsAsync((Usuario u) => u);
            await _service.CadastrarAsync(new CadastroUsuarioDto { Nome = "Ana", Email = "ana@teste.com", Senha = "123456" });
            _mockRepository.Setup(r => r.BuscarPorEmailAsync("ana@teste.com")).ReturnsAsync(usuarioSalvo);

            // Act
            var resposta = await _service.LoginAsync(new LoginDto { Email = "ana@teste.com", Senha = "123456" });

            // Assert
            Assert.NotNull(resposta);
            Assert.Equal("token-de-teste", resposta.Token);
        }

        [Fact]
        public async Task ExcluirConta_UsuarioExiste_ExcluiERetornaTrue()
        {
            // Arrange
            var usuario = new Usuario { Id = 7, Nome = "Ana", Email = "ana@teste.com", Tipo = "USUARIO" };
            _mockRepository.Setup(r => r.BuscarPorIdAsync(7)).ReturnsAsync(usuario);

            // Act
            var excluida = await _service.ExcluirContaAsync(7);

            // Assert
            Assert.True(excluida);
            _mockRepository.Verify(r => r.ExcluirAsync(usuario), Times.Once);
        }

        [Fact]
        public async Task ExcluirConta_UsuarioAdmin_LancaInvalidOperationException()
        {
            // Arrange
            var admin = new Usuario { Id = 1, Nome = "Admin", Email = "admin@teste.com", Tipo = "ADMIN" };
            _mockRepository.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync(admin);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ExcluirContaAsync(1));
            _mockRepository.Verify(r => r.ExcluirAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task ExcluirConta_UsuarioNaoExiste_RetornaFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Usuario?)null);

            // Act
            var excluida = await _service.ExcluirContaAsync(99);

            // Assert
            Assert.False(excluida);
            _mockRepository.Verify(r => r.ExcluirAsync(It.IsAny<Usuario>()), Times.Never);
        }
    }
}

