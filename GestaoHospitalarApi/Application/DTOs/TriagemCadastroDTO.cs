namespace GestaoHospitalarApi.Application.DTOs
{
    public class TriagemCadastroDto
    {
        // Dados da Triagem
        public int IdPaciente { get; set; }
        public string? ResponsavelTriagem { get; set; }
        public string? Pressao { get; set; }
        public decimal? Temperatura { get; set; }
        public int? FrequenciaCardiaca { get; set; }
        public int? Saturacao { get; set; }
        public int? EscalaDor { get; set; }
        public string? Risco { get; set; } // VERMELHO, LARANJA, AMARELO, VERDE, AZUL
        public string? Queixa { get; set; }
        public string? Alergias { get; set; }
        public string? Observacoes { get; set; }
        public string? Internacao { get; set; } // SIM ou NAO

        // Dados Opcionais do Convênio (preenchidos na tela de Triagem)
        public int? IdConvenio { get; set; }
        public string? NumeroCarteira { get; set; }
        public DateTime? ValidadeConvenio { get; set; }
    }
}