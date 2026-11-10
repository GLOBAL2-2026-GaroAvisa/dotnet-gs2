using System.ComponentModel.DataAnnotations;

namespace GaroAvisa.Domain.Entities
{
    public class RegistroIdempotencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Chave { get; set; } = string.Empty;

        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Rota { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public string HashRequisicao { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string RespostaJson { get; set; } = string.Empty;

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}