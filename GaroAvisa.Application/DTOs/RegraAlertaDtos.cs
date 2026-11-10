using System.ComponentModel.DataAnnotations;

namespace GaroAvisa.Application.DTOs
{
    public class SalvarRegraAlertaDto
    {
        [Required(ErrorMessage = "Informe o tipo da regra.")]
        [RegularExpression("^(CHUVA_FORTE|VENTO_FORTE|CALOR|FRIO|TEMPESTADE)$", ErrorMessage = "Tipo inválido. Use CHUVA_FORTE, VENTO_FORTE, CALOR, FRIO ou TEMPESTADE.")]
        public string Tipo { get; set; } = string.Empty;

        [Range(-50, 500, ErrorMessage = "O limite deve ficar entre -50 e 500.")]
        public double Limite { get; set; }

        [Required(ErrorMessage = "Informe a gravidade.")]
        [RegularExpression("^(BAIXA|MEDIA|ALTA)$", ErrorMessage = "Gravidade inválida. Use BAIXA, MEDIA ou ALTA.")]
        public string Gravidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o título.")]
        [MaxLength(100, ErrorMessage = "O título pode ter até 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a descrição.")]
        [MaxLength(300, ErrorMessage = "A descrição pode ter até 300 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        public bool Ativa { get; set; } = true;
    }

    public class RegraAlertaRespostaDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public double Limite { get; set; }
        public string Gravidade { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Ativa { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}