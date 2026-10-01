namespace GestaoHospitalarApi.Application.DTOs.Medico
{
    public class MedicoCadastroDto
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

        // --- Dados Específicos do Médico ---
        public string Crm { get; set; } = string.Empty; // Será usado também como Login do usuário
        public decimal Honorario { get; set; }
        
        // O front envia a descrição selecionada na lista, não o ID
        public string DescricaoEspecialidade { get; set; } = string.Empty;

        // --- Dados de Acesso / Login ---
        public string Perfil { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}