using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GaroAvisa.Domain.Entities
{
    public class Preferencia
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [JsonIgnore]
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public bool ResumoDiarioAtivo { get; set; } = true;

        [Required]
        [MaxLength(5)]
        public string ResumoDiarioHorario { get; set; } = "07:00";

        public bool AlertasDoDia { get; set; } = true;
        public bool PrevisaoMudou { get; set; } = true;
        public bool Compromissos { get; set; } = true;
        public bool SilencioNoturno { get; set; } = true;

        public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
    }
}