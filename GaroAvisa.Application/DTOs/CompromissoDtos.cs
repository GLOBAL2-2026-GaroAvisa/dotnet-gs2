using System.ComponentModel.DataAnnotations;

namespace GaroAvisa.Application.DTOs
{
    public class SalvarCompromissoDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Informe o local do compromisso.")]
        public int LocalId { get; set; }

        [Required(ErrorMessage = "Informe o nome do compromisso.")]
        [MaxLength(80, ErrorMessage = "O nome pode ter até 80 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a data e hora do compromisso.")]
        public DateTime? DataHora { get; set; }
    }

    public class CompromissoRespostaDto
    {
        public int Id { get; set; }
        public int LocalId { get; set; }
        public string LocalEtiqueta { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public DateTime CriadoEm { get; set; }
    }

    public class CompromissoPrevisaoDto
    {
        public CompromissoRespostaDto Compromisso { get; set; } = new();

        public PrevisaoDiaDto? PrevisaoDoDia { get; set; }
        public List<AlertaRespostaDto> AlertasDoDia { get; set; } = new();
        public string? Mensagem { get; set; }
    }
}