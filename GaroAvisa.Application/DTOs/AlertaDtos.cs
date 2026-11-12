namespace GaroAvisa.Application.DTOs
{
    public class AlertaRespostaDto
    {
        public int Id { get; set; }
        public int LocalId { get; set; }
        public string LocalEtiqueta { get; set; } = string.Empty;
        public string DataPrevisao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Gravidade { get; set; } = string.Empty;
        public double? ValorPrevisto { get; set; }
        public bool Lido { get; set; }
        public DateTime CriadoEm { get; set; }
    }

    public class VerificacaoResultadoDto
    {
        public int LocaisVerificados { get; set; }
        public int LocaisComFalha { get; set; }
        public int AlertasCriados { get; set; }
        public int AlertasPrevisaoMudou { get; set; }
        public DateTime ExecutadoEm { get; set; }
    }

    public class ResumoLocalDto
    {
        public int LocalId { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;

        public PrevisaoDiaDto? PrevisaoHoje { get; set; }
        public List<AlertaRespostaDto> AlertasHoje { get; set; } = new();
    }

    public class ResumoDiaDto
    {
        public string Data { get; set; } = string.Empty;
        public bool ResumoDiarioAtivo { get; set; }
        public string ResumoDiarioHorario { get; set; } = string.Empty;
        public int TotalAlertasHoje { get; set; }
        public List<ResumoLocalDto> Locais { get; set; } = new();
    }
}