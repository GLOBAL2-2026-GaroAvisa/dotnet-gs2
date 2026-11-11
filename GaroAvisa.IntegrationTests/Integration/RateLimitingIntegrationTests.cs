using System.Net;
using System.Net.Http.Json;
using GaroAvisa.IntegrationTests.FactoryFixture;

namespace GaroAvisa.IntegrationTests.Integration
{
    // Usa uma API separada, com limite de só 3 logins por minuto.
    public class RateLimitingIntegrationTests : IClassFixture<ApiFactoryComLimiteBaixo>
    {
        private readonly HttpClient _client;

        public RateLimitingIntegrationTests(ApiFactoryComLimiteBaixo factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_MuitasTentativas_RetornaTooManyRequests()
        {
            // Arrange
            var tentativa = new { email = "ninguem@teste.com", senha = "errada" };
            var codigos = new List<HttpStatusCode>();

            // Act: 4 tentativas seguidas, com limite de 3
            for (int i = 0; i < 4; i++)
            {
                var response = await _client.PostAsJsonAsync("/api/Auth/login", tentativa);
                codigos.Add(response.StatusCode);
            }

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, codigos[0]);
            Assert.Equal(HttpStatusCode.TooManyRequests, codigos[3]);
        }
    }
}
