using System.Globalization;
using System.Net.Http.Json;
using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Helpers;
using GaroAvisa.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GaroAvisa.Infrastructure.ExternalServices
{
    public class OpenMeteoClient : IOpenMeteoClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _geocodingUrl;
        private readonly string _forecastUrl;

        public OpenMeteoClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _geocodingUrl = configuration["OpenMeteo:GeocodingUrl"]!;
            _forecastUrl = configuration["OpenMeteo:ForecastUrl"]!;
        }

        public async Task<List<LugarDto>> BuscarLugaresAsync(string nome)
        {
            var url = $"{_geocodingUrl}?name={Uri.EscapeDataString(nome)}&count=10&language=pt&format=json";

            var resposta = await _httpClient.GetFromJsonAsync<GeocodingResposta>(url);

            if (resposta?.Results is null)
            {
                return new List<LugarDto>();
            }

            return resposta.Results.Select(l => new LugarDto
            {
                Nome = l.Name,
                Estado = l.Admin1,
                Pais = l.Country,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                Fuso = l.Timezone ?? "America/Sao_Paulo"
            }).ToList();
        }

        public async Task<List<PrevisaoDiaDto>> ObterPrevisaoAsync(double latitude, double longitude, string fuso, int dias)
        {
            // InvariantCulture garante o ponto como separador decimal (-23.54 e não -23,54).
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);

            var url = $"{_forecastUrl}?latitude={lat}&longitude={lon}" +
                      "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum,precipitation_probability_max,wind_gusts_10m_max" +
                      $"&timezone={Uri.EscapeDataString(fuso)}&forecast_days={dias}";

            var resposta = await _httpClient.GetFromJsonAsync<ForecastResposta>(url);
            var diario = resposta?.Daily;

            if (diario is null)
            {
                return new List<PrevisaoDiaDto>();
            }

            var previsao = new List<PrevisaoDiaDto>();

            for (int i = 0; i < diario.Time.Count; i++)
            {
                var codigo = diario.WeatherCode.ElementAtOrDefault(i) ?? 0;

                previsao.Add(new PrevisaoDiaDto
                {
                    Data = diario.Time[i],
                    CodigoTempo = codigo,
                    Descricao = CodigoTempo.Descrever(codigo),
                    TemperaturaMaxima = diario.TemperatureMax.ElementAtOrDefault(i),
                    TemperaturaMinima = diario.TemperatureMin.ElementAtOrDefault(i),
                    ChuvaMm = diario.PrecipitationSum.ElementAtOrDefault(i),
                    ProbabilidadeChuva = diario.PrecipitationProbabilityMax.ElementAtOrDefault(i),
                    RajadaVentoKmh = diario.WindGustsMax.ElementAtOrDefault(i)
                });
            }

            return previsao;
        }
    }
}