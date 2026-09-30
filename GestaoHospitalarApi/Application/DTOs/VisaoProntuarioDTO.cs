namespace GestaoHospitalarApi.Application.DTOs
{
    public class VisaoProntuarioDto
    {
        public int IdProntuario { get; set; } 
        public int IdMedico { get; set; } 
        public string NomeMedico { get; set; } = string.Empty; // <-- Nome adicionado

        // Dados do Paciente
        public string NomePaciente { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public string DataAdmissao { get; set; } = string.Empty; // <-- Transformado em string para formatar

        // Informações Clínicas Importantes
        public string Alergias { get; set; } = "Nenhuma relatada";
        public string RiscoEvasao { get; set; } = "NAO";
        public string Isolamento { get; set; } = "NAO";

        // Sinais Vitais
        public string? Pressao { get; set; }
        public decimal? Temperatura { get; set; }
        public int? Saturacao { get; set; }
        public string? QueixaPrincipal { get; set; }

        // Evolução
        public string? Evolucao { get; set; }
    }
}