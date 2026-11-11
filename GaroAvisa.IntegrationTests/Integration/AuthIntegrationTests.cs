using System.Net;
using System.Net.Http.Json;
using GaroAvisa.Application.DTOs;
using GaroAvisa.IntegrationTests.FactoryFixture;

namespace GaroAvisa.IntegrationTests.Integration
{
    [Collection("ApiCollection")]
    public class AuthIntegrationTests
    {
        private readonly ApiFactoryFixture _factory;
        private readonly HttpClient _client;

        public AuthIntegrationTests(ApiFactoryFixture factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Cadastro_DadosValidos_RetornaCreatedComToken()
        {
            // Arrange
            var novoUsuario = new { nome = "Ana", email = $"ana-{Guid.NewGuid():N}@teste.com", senha = "123456" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/cadastro", novoUsuario);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var resposta = await response.Content.ReadFromJsonAsync<LoginRespostaDto>();
            Assert.NotNull(resposta);
            Assert.False(string.IsNullOrWhiteSpace(resposta.Token));
            Assert.Equal("USUARIO", resposta.Usuario.Tipo);
        }

        [Fact]
        public async Task Cadastro_EmailRepetido_RetornaConflict()
        {
            // Arrange
            var usuario = new { nome = "Ana", email = $"repetido-{Guid.NewGuid():N}@teste.com", senha = "123456" };
            await _client.PostAsJsonAsync("/api/Auth/cadastro", usuario);

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/cadastro", usuario);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Cadastro_EmailInvalido_RetornaBadRequest()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/cadastro", new { nome = "Ana", email = "nao-e-email", senha = "123" });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_SenhaErrada_RetornaUnauthorized()
        {
            // Arrange
            var email = $"login-{Guid.NewGuid():N}@teste.com";
            await _client.PostAsJsonAsync("/api/Auth/cadastro", new { nome = "Ana", email, senha = "123456" });

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", new { email, senha = "errada" });

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task HealthLive_RetornaOk()
        {
            // Act
            var response = await _client.GetAsync("/health/live");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
