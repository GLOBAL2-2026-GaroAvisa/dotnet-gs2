using System.ComponentModel.DataAnnotations;

namespace GaroAvisa.Application.DTOs
{
    public class SalvarPreferenciaDto
    {
        public bool ResumoDiarioAtivo { get; set; } = true;

        [Required(ErrorMessage = "Informe o horário do resumo.")]
        [RegularExpression(@"^([01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "O horário deve estar no formato HH:mm, por exemplo 07:00.")]
        public string ResumoDiarioHorario { get; set; } = "07:00";

        public bool AlertasDoDia { get; set; } = true;
        public bool PrevisaoMudou { get; set; } = true;
        public bool Compromissos { get; set; } = true;
        public bool SilencioNoturno { get; set; } = true;
    }

    public class PreferenciaRespostaDto
    {
        public bool ResumoDiarioAtivo { get; set; }
        public string ResumoDiarioHorario { get; set; } = string.Empty;
        public bool AlertasDoDia { get; set; }
        public bool PrevisaoMudou { get; set; }
        public bool Compromissos { get; set; }
        public bool SilencioNoturno { get; set; }

        // false quando o usuário ainda não salvou nada e está usando o padrão
        public bool Personalizada { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}