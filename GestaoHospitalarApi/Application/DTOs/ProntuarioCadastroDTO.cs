namespace GestaoHospitalarApi.Application.DTOs
{
    public class ProntuarioCadastroDto
    {
        public int IdPaciente { get; set; }
        public int IdTriagem { get; set; }
        public int IdMedico { get; set; }
        public int? IdSala { get; set; }
        public string RiscoEvasao { get; set; } = "NAO"; // SIM ou NAO
        public string? Evolucao { get; set; }
        public string StatusProntuario { get; set; } = "ATIVO"; // ATIVO ou ARQUIVADO
    }
}