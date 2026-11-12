using System.Net;
using System.Net.Http.Json;
using GaroAvisa.Application.DTOs;
using GaroAvisa.IntegrationTests.FactoryFixture;

namespace GaroAvisa.IntegrationTests.Integration
{
    [Collection("ApiCollection")]
    public class LocaisIntegrationTests
    {
        private readonly ApiFactoryFixture _factory;

        public LocaisIntegrationTests(ApiFactoryFixture factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Listar_SemToken_RetornaUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/Locais");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Crud_CompletoDeUmLocal_FuncionaDoInicioAoFim()
        {
            // Arrange
            var client = await TestHelper.CriarClienteLogadoAsync(_factory);

            // Act + Assert: criar
            var criado = await TestHelper.CriarLocalAsync(client);
            Assert.Equal("Casa", criado.Etiqueta);

            // buscar
            var busca = await client.GetAsync($"/api/Locais/{criado.Id}");
            Assert.Equal(HttpStatusCode.OK, busca.StatusCode);

            // atualizar
            var atualizacao = await client.PutAsJsonAsync($"/api/Locais/{criado.Id}", new { etiqueta = "Minha casa", principal = true, receberAlertas = false });
            Assert.Equal(HttpStatusCode.OK, atualizacao.StatusCode);
            var atualizado = await atualizacao.Content.ReadFromJsonAsync<LocalRespostaDto>();
            Assert.Equal("Minha casa", atualizado!.Etiqueta);
            Assert.False(atualizado.ReceberAlertas);

            // excluir
            var exclusao = await client.DeleteAsync($"/api/Locais/{criado.Id}");
            Assert.Equal(HttpStatusCode.OK, exclusao.StatusCode);

            // depois de excluir, não encontra mais
            var buscaDepois = await client.GetAsync($"/api/Locais/{criado.Id}");
            Assert.Equal(HttpStatusCode.NotFound, buscaDepois.StatusCode);
        }

        [Fact]
        public async Task Criar_LatitudeInvalida_RetornaBadRequest()
        {
            // Arrange
            var client = await TestHelper.CriarClienteLogadoAsync(_factory);
            var localInvalido = new { etiqueta = "Lua", nome = "Lugar", latitude = 200, longitude = 0 };

            // Act
            var response = await client.PostAsJsonAsync("/api/Locais", localInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Buscar_LocalDeOutroUsuario_RetornaNotFound()
        {
            // Arrange
            var dono = await TestHelper.CriarClienteLogadoAsync(_factory);
            var outro = await TestHelper.CriarClienteLogadoAsync(_factory);
            var local = await TestHelper.CriarLocalAsync(dono);

            // Act
            var response = await outro.GetAsync($"/api/Locais/{local.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Criar_MesmaIdempotencyKeyDuasVezes_CriaUmaVezSo()
        {
            // Arrange
            var client = await TestHelper.CriarClienteLogadoAsync(_factory);
            var local = new { etiqueta = "Escola", nome = "Mooca", latitude = -23.55, longitude = -46.60 };

            HttpRequestMessage NovoPedido()
            {
                var pedido = new HttpRequestMessage(HttpMethod.Post, "/api/Locais") { Content = JsonContent.Create(local) };
                pedido.Headers.Add("Idempotency-Key", "chave-teste-001");
                return pedido;
            }

            // Act
            var primeira = await client.SendAsync(NovoPedido());
            var segunda = await client.SendAsync(NovoPedido());

            // Assert
            Assert.Equal(HttpStatusCode.Created, primeira.StatusCode);
            Assert.Equal(HttpStatusCode.Created, segunda.StatusCode);
            Assert.True(segunda.Headers.Contains("Idempotent-Replay"));

            var criado1 = await primeira.Content.ReadFromJsonAsync<LocalRespostaDto>();
            var criado2 = await segunda.Content.ReadFromJsonAsync<LocalRespostaDto>();
            Assert.Equal(criado1!.Id, criado2!.Id);

            var lista = await client.GetFromJsonAsync<List<LocalRespostaDto>>("/api/Locais");
            Assert.Single(lista!);
        }

        [Fact]
        public async Task Criar_MesmaIdempotencyKeyComOutrosDados_RetornaUnprocessableEntity()
        {
            // Arrange
            var client = await TestHelper.CriarClienteLogadoAsync(_factory);

            var pedido1 = new HttpRequestMessage(HttpMethod.Post, "/api/Locais")
            {
                Content = JsonContent.Create(new { etiqueta = "Escola", nome = "Mooca", latitude = -23.55, longitude = -46.60 })
            };
            pedido1.Headers.Add("Idempotency-Key", "chave-teste-002");

            var pedido2 = new HttpRequestMessage(HttpMethod.Post, "/api/Locais")
            {
                Content = JsonContent.Create(new { etiqueta = "Academia", nome = "Tatuapé", latitude = -23.54, longitude = -46.57 })
            };
            pedido2.Headers.Add("Idempotency-Key", "chave-teste-002");

            // Act
            await client.SendAsync(pedido1);
            var response = await client.SendAsync(pedido2);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
    }
}
