using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GaroAvisa.Domain.Entities
{
    public class PrevisaoSalva
    {
        [Key]
        public int Id { get; set; }

        public int LocalId { get; set; }

        [JsonIgnore]
        [ForeignKey("LocalId")]
        public Local? Local { get; set; }

        public DateTime DataPrevisao { get; set; }

        public int CodigoTempo { get; set; }
        public double? TemperaturaMaxima { get; set; }
        public double? TemperaturaMinima { get; set; }
        public double? ChuvaMm { get; set; }
        public int? ProbabilidadeChuva { get; set; }
        public double? RajadaVentoKmh { get; set; }

        public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
    }
}