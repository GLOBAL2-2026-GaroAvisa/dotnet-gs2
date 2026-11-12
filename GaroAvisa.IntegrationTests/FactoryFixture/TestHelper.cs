using System.Net.Http.Headers;
using System.Net.Http.Json;
using GaroAvisa.Application.DTOs;
using GaroAvisa.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GaroAvisa.IntegrationTests.FactoryFixture
{
    public static class TestHelper
    {
        // Cria um usuário novo (e-mail único) e devolve um HttpClient já com o token.
        public static async Task<HttpClient> CriarClienteLogadoAsync(ApiFactoryFixture factory)
        {
            var client = factory.CreateClient();
            var email = $"usuario-{Guid.NewGuid():N}@teste.com";

            var resposta = await client.PostAsJsonAsync("/api/Auth/cadastro", new { nome = "Teste", email, senha = "123456" });
            resposta.EnsureSuccessStatusCode();

            var login = await resposta.Content.ReadFromJsonAsync<LoginRespostaDto>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
            return client;
        }

        // Cria um usuário, promove para ADMIN direto no banco (como faríamos no Oracle) e faz login de novo.
        public static async Task<HttpClient> CriarClienteAdminAsync(ApiFactoryFixture factory)
        {
            var client = factory.CreateClient();
            var email = $"admin-{Guid.NewGuid():N}@teste.com";

            var cadastro = await client.PostAsJsonAsync("/api/Auth/cadastro", new { nome = "Admin", email, senha = "123456" });
            cadastro.EnsureSuccessStatusCode();

            using (var scope = factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var usuario = context.Usuarios.First(u => u.Email == email);
                usuario.Tipo = "ADMIN";
                context.SaveChanges();
            }

            var login = await client.PostAsJsonAsync("/api/Auth/login", new { email, senha = "123456" });
            var dados = await login.Content.ReadFromJsonAsync<LoginRespostaDto>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", dados!.Token);
            return client;
        }

        public static async Task<LocalRespostaDto> CriarLocalAsync(HttpClient client, string etiqueta = "Casa")
        {
            var resposta = await client.PostAsJsonAsync("/api/Locais", new
            {
                etiqueta,
                nome = "Tatuapé",
                estado = "SP",
                pais = "Brasil",
                latitude = -23.54,
                longitude = -46.57,
                principal = true,
                receberAlertas = true
            });
            resposta.EnsureSuccessStatusCode();
            return (await resposta.Content.ReadFromJsonAsync<LocalRespostaDto>())!;
        }
    }
}
