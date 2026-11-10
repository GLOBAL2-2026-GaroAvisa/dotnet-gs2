using System.ComponentModel.DataAnnotations;

namespace GaroAvisa.Domain.Entities
{
    public class RegraAlerta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Tipo { get; set; } = string.Empty;

        public double Limite { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gravidade { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; } = string.Empty;

        public bool Ativa { get; set; } = true;

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}