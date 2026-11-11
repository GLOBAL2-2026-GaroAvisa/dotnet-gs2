using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GaroAvisa.IntegrationTests.FactoryFixture
{
    // Sobe a API inteira em memória para os testes:
    // troca o Oracle por um banco em memória e a Open-Meteo por uma versão falsa,
    // assim os testes rodam em qualquer máquina (e no pipeline) sem internet nem banco.
    public class ApiFactoryFixture : WebApplicationFactory<Program>
    {
        protected virtual int LoginsPorMinuto => 1000;

        private readonly string _nomeBanco = $"GaroAvisaTestes-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Ambiente "Testing": não lê o appsettings.Development.json (que tem a senha real)
            builder.UseEnvironment("Testing");

            builder.UseSetting("Jwt:Key", "chave-somente-para-testes-com-mais-de-32-caracteres");
            builder.UseSetting("RateLimiting:PedidosPorMinuto", "1000");
            builder.UseSetting("RateLimiting:LoginsPorMinuto", LoginsPorMinuto.ToString());

            builder.ConfigureServices(services =>
            {
                // 1. Remove o Oracle e coloca o banco em memória
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_nomeBanco));

                // 2. Troca a Open-Meteo de verdade por uma falsa, com respostas fixas
                services.RemoveAll<IOpenMeteoClient>();
                services.AddSingleton<IOpenMeteoClient, OpenMeteoFalso>();
            });
        }
    }

    // Versão do factory com limite baixo de login, para testar o 429
    public class ApiFactoryComLimiteBaixo : ApiFactoryFixture
    {
        protected override int LoginsPorMinuto => 3;
    }

    public class OpenMeteoFalso : IOpenMeteoClient
    {
        public Task<List<LugarDto>> BuscarLugaresAsync(string nome)
        {
            var lugares = new List<LugarDto>
            {
                new LugarDto { Nome = "Tatuapé", Estado = "São Paulo", Pais = "Brasil", Latitude = -23.54, Longitude = -46.57, Fuso = "America/Sao_Paulo" }
            };
            return Task.FromResult(lugares);
        }

        public Task<List<PrevisaoDiaDto>> ObterPrevisaoAsync(double latitude, double longitude, string fuso, int dias)
        {
            var previsao = new List<PrevisaoDiaDto>();
            for (int i = 0; i < dias; i++)
            {
                previsao.Add(new PrevisaoDiaDto
                {
                    Data = DateTime.UtcNow.AddHours(-3).Date.AddDays(i).ToString("yyyy-MM-dd"),
                    CodigoTempo = 95,
                    Descricao = "Trovoada",
                    TemperaturaMaxima = 35,
                    TemperaturaMinima = 19,
                    ChuvaMm = 30,
                    ProbabilidadeChuva = 90,
                    RajadaVentoKmh = 40
                });
            }
            return Task.FromResult(previsao);
        }
    }

    // Collection: compartilha a mesma API entre as classes de teste
    [CollectionDefinition("ApiCollection")]
    public class ApiCollection : ICollectionFixture<ApiFactoryFixture>
    {
    }
}
