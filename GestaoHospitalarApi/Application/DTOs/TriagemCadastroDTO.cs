namespace GestaoHospitalarApi.Application.DTOs
{
    public class TriagemCadastroDto
    {
        // Alterado de NomePaciente para CpfPaciente
        public string CpfPaciente { get; set; } = string.Empty;

        // --- Sinais Vitais ---
        public string PressaoArterial { get; set; } = string.Empty;
        public decimal Temperatura { get; set; }
        public int FrequenciaCardiaca { get; set; }
        public int Saturacao { get; set; }
        
        // --- Avaliação Clínica ---
        public string QueixaPrincipal { get; set; } = string.Empty;
        public int EscalaDor { get; set; }
        public string? AlergiasRelatadas { get; set; } 
        public string? Observacoes { get; set; }

        // --- Classificação de Risco ---
        public string ClassificacaoRisco { get; set; } = string.Empty; 
    }
}