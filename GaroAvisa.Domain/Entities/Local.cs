using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GaroAvisa.Domain.Entities
{
    public class Local
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [JsonIgnore]
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required]
        [MaxLength(60)]
        public string Etiqueta { get; set; } = string.Empty;

        [Required]
        [MaxLength(120)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Estado { get; set; }

        [MaxLength(80)]
        public string? Pais { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required]
        [MaxLength(60)]
        public string Fuso { get; set; } = "America/Sao_Paulo";

        public bool Principal { get; set; }
        public bool ReceberAlertas { get; set; } = true;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
