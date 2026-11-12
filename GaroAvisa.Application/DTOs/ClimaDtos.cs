namespace GaroAvisa.Application.DTOs
{
    public class LugarDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Fuso { get; set; } = string.Empty;
    }

    public class PrevisaoDiaDto
    {
        public string Data { get; set; } = string.Empty;
        public int CodigoTempo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public double? TemperaturaMaxima { get; set; }
        public double? TemperaturaMinima { get; set; }
        public double? ChuvaMm { get; set; }
        public int? ProbabilidadeChuva { get; set; }
        public double? RajadaVentoKmh { get; set; }
    }

    public class PrevisaoLocalDto
    {
        public int LocalId { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public List<PrevisaoDiaDto> Dias { get; set; } = new();
    }
}