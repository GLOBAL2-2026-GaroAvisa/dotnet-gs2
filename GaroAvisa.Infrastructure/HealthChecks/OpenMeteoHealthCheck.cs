using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GaroAvisa.Infrastructure.HealthChecks
{
    public class OpenMeteoHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _forecastUrl;

        public OpenMeteoHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _forecastUrl = configuration["OpenMeteo:ForecastUrl"]!;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            var url = $"{_forecastUrl}?latitude=-23.55&longitude=-46.63&current=temperature_2m";
            var cronometro = Stopwatch.StartNew();

            try
            {
                var resposta = await httpClient.GetAsync(url, cancellationToken);
                cronometro.Stop();

                if (resposta.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy($"Open-Meteo respondeu em {cronometro.ElapsedMilliseconds} ms.");
                }

                return HealthCheckResult.Degraded($"Open-Meteo respondeu com o código {(int)resposta.StatusCode}.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Degraded("Open-Meteo não respondeu.", ex);
            }
        }
    }
}