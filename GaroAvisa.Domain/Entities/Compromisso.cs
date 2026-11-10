using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GaroAvisa.Domain.Entities
{
    public class Compromisso
    {
        [Key]
        public int Id { get; set; }

        public int LocalId { get; set; }

        [JsonIgnore]
        [ForeignKey("LocalId")]
        public Local? Local { get; set; }

        [Required]
        [MaxLength(80)]
        public string Nome { get; set; } = string.Empty;

        public DateTime DataHora { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}