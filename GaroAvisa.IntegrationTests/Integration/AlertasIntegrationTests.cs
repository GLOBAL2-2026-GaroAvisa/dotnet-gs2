using System.Net;
using System.Net.Http.Json;
using GaroAvisa.Application.DTOs;
using GaroAvisa.IntegrationTests.FactoryFixture;

namespace GaroAvisa.IntegrationTests.Integration
{
    [Collection("ApiCollection")]
    public class AlertasIntegrationTests
    {
        private readonly ApiFactoryFixture _factory;

        public AlertasIntegrationTests(ApiFactoryFixture factory)
        {
            _factory = factory;
        }

        private static object RegraDeChuva(string gravidade)
        {
            return new { tipo = "CHUVA_FORTE", limite = 20, gravidade, titulo = "Chuva forte", descricao = "Evite alagamentos.", ativa = true };
        }

        [Fact]
        public async Task CriarRegra_UsuarioComum_RetornaForbidden()
        {
            // Arrange
            var client = await TestHelper.CriarClienteLogadoAsync(_factory);

            // Act
            var response = await client.PostAsJsonAsync("/api/RegrasAlerta", RegraDeChuva("BAIXA"));

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CriarRegra_Admin_RetornaCreated()
        {
            // Arrange
            var admin = await TestHelper.CriarClienteAdminAsync(_factory);

            // Act
            var response = await admin.PostAsJsonAsync("/api/RegrasAlerta", new
            {
                tipo = "VENTO_FORTE", limite = 50, gravidade = "MEDIA", titulo = "Ventos fortes", descricao = "Cuidado.", ativa = true
            });

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CriarRegra_TipoInvalido_RetornaBadRequest()
        {
            // Arrange
            var admin = await TestHelper.CriarClienteAdminAsync(_factory);

            // Act
            var response = await admin.PostAsJsonAsync("/api/RegrasAlerta", new
            {
                tipo = "GRANIZO", limite = 1, gravidade = "ALTA", titulo = "x", descricao = "y"
            });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Verificar_PrevisaoComChuvaForte_CriaAlertaParaODonoDoLocal()
        {
            // Arrange: admin cria a regra, usuário cadastra o local
            var admin = await TestHelper.CriarClienteAdminAsync(_factory);
            await admin.PostAsJsonAsync("/api/RegrasAlerta", RegraDeChuva("ALTA"));

            var usuario = await TestHelper.CriarClienteLogadoAsync(_factory);
            var local = await TestHelper.CriarLocalAsync(usuario);

            // Act: admin verifica (a Open-Meteo falsa devolve 30 mm de chuva)
            var verificacao = await admin.PostAsync("/api/Alertas/verificar", null);
            var alertas = await usuario.GetFromJsonAsync<List<AlertaRespostaDto>>("/api/Alertas");

            // Assert
            Assert.Equal(HttpStatusCode.OK, verificacao.StatusCode);
            Assert.Contains(alertas!, a => a.LocalId == local.Id && a.Tipo == "CHUVA_FORTE");
        }

        [Fact]
        public async Task Verificar_UsuarioComum_RetornaForbidden()
        {
            // Arrange
            var client = await TestHelper.CriarClienteLogadoAsync(_factory);

            // Act
            var response = await client.PostAsync("/api/Alertas/verificar", null);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Previsao_LocalDoUsuario_RetornaDiasTraduzidos()
        {
            // Arrange
            var client = await TestHelper.CriarClienteLogadoAsync(_factory);
            var local = await TestHelper.CriarLocalAsync(client);

            // Act
            var previsao = await client.GetFromJsonAsync<PrevisaoLocalDto>($"/api/Previsao/{local.Id}?dias=3");

            // Assert
            Assert.NotNull(previsao);
            Assert.Equal(3, previsao.Dias.Count);
            Assert.Equal("Trovoada", previsao.Dias[0].Descricao);
        }

        [Fact]
        public async Task Previsao_DiasForaDoLimite_RetornaBadRequest()
        {
            // Arrange
            var client = await TestHelper.CriarClienteLogadoAsync(_factory);
            var local = await TestHelper.CriarLocalAsync(client);

            // Act
            var response = await client.GetAsync($"/api/Previsao/{local.Id}?dias=30");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
