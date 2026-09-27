namespace GestaoHospitalarApi.DTOs
{
    public class TriagemCreateDTO
    {
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
    }
}