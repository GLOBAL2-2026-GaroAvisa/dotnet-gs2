using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GaroAvisa.Domain.Entities
{
    public class Alerta
    {
        [Key]
        public int Id { get; set; }

        public int LocalId { get; set; }

        [JsonIgnore]
        [ForeignKey("LocalId")]
        public Local? Local { get; set; }

        public DateTime DataPrevisao { get; set; }

        [Required]
        [MaxLength(30)]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Gravidade { get; set; } = string.Empty;

        public double? ValorPrevisto { get; set; }

        public bool Lido { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}