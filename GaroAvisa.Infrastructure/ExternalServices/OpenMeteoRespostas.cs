using System.Text.Json.Serialization;

namespace GaroAvisa.Infrastructure.ExternalServices
{

    public class GeocodingResposta
    {
        [JsonPropertyName("results")]
        public List<GeocodingLugar>? Results { get; set; }
    }

    public class GeocodingLugar
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("admin1")]
        public string? Admin1 { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }
    }

    public class ForecastResposta
    {
        [JsonPropertyName("daily")]
        public ForecastDiario? Daily { get; set; }
    }

    public class ForecastDiario
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new();

        [JsonPropertyName("weather_code")]
        public List<int?> WeatherCode { get; set; } = new();

        [JsonPropertyName("temperature_2m_max")]
        public List<double?> TemperatureMax { get; set; } = new();

        [JsonPropertyName("temperature_2m_min")]
        public List<double?> TemperatureMin { get; set; } = new();

        [JsonPropertyName("precipitation_sum")]
        public List<double?> PrecipitationSum { get; set; } = new();

        [JsonPropertyName("precipitation_probability_max")]
        public List<int?> PrecipitationProbabilityMax { get; set; } = new();

        [JsonPropertyName("wind_gusts_10m_max")]
        public List<double?> WindGustsMax { get; set; } = new();
    }
}