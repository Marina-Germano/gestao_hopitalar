namespace GestaoHospitalarApi.Application.DTOs
{
    public class PacienteCadastroDto
    {
        // --- Dados Pessoais ---
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string? Email { get; set; }

        // --- Dados de Endereço ---
        public string Cep { get; set; } = string.Empty;
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        // --- Dados Médicos do Paciente ---
        public string? Alergias { get; set; }
        public string? TipoSanguineo { get; set; }
        public string? HistoricoClinico { get; set; }
        public string? NomeResponsavel { get; set; }

        // --- Dados do Convênio ---
        // Pode ser nulo caso o paciente seja de atendimento particular
        public PacienteConvenioCadastroDto? Convenio { get; set; }
    }

    public class PacienteConvenioCadastroDto
    {
        // Recebe o nome em vez do ID, conforme selecionado na lista do frontend
        public string NomeConvenio { get; set; } = string.Empty; 
        public string Numero { get; set; } = string.Empty;
        public DateTime Validade { get; set; }
    }
}