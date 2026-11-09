using System.ComponentModel.DataAnnotations;

namespace GaroAvisa.Application.DTOs
{
    public class CriarLocalDto
    {
        [Required(ErrorMessage = "Informe a etiqueta do local.")]
        [MaxLength(60, ErrorMessage = "A etiqueta pode ter até 60 caracteres.")]
        public string Apelido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o nome do lugar.")]
        [MaxLength(120, ErrorMessage = "O nome pode ter até 120 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Estado { get; set; }

        [MaxLength(80)]
        public string? Pais { get; set; }

        [Range(-90, 90, ErrorMessage = "A latitude deve ficar entre -90 e 90.")]
        public double Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "A longitude deve ficar entre -180 e 180.")]
        public double Longitude { get; set; }

        [MaxLength(60)]
        public string Fuso { get; set; } = "America/Sao_Paulo";

        public bool Principal { get; set; }
        public bool ReceberAlertas { get; set; } = true;
    }

    public class AtualizarLocalDto
    {
        [Required(ErrorMessage = "Informe a etiqueta do local.")]
        [MaxLength(60, ErrorMessage = "A etiqueta pode ter até 60 caracteres.")]
        public string Apelido { get; set; } = string.Empty;

        public bool Principal { get; set; }
        public bool ReceberAlertas { get; set; } = true;
    }

    public class LocalRespostaDto
    {
        public int Id { get; set; }
        public string Apelido { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Fuso { get; set; } = string.Empty;
        public bool Principal { get; set; }
        public bool ReceberAlertas { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
