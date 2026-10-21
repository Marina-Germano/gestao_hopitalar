namespace GestaoHospitalarApi.Application.DTOs
{
    public class TriagemListagemDto
    {
        public int IdTriagem { get; set; }
        
        // --- Dados do Paciente ---
        public string NomePaciente { get; set; } = string.Empty;
        public string CpfPaciente { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        
        // --- Dados da Triagem ---
        public string ClassificacaoRisco { get; set; } = string.Empty; // Cor
        public string QueixaPrincipal { get; set; } = string.Empty;
        
        // (A data da triagem é útil para o front ordenar quem chegou primeiro)
        public DateTime DataTriagem { get; set; } 
    }
}