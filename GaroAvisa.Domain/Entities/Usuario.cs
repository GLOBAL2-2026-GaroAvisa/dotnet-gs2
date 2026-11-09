using System.ComponentModel.DataAnnotations;

namespace GaroAvisa.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Tipo { get; set; } = "CIDADAO";

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
