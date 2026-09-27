namespace GestaoHospitalarApi.DTOs
{
    public class MedicoCreateDTO
    {
        // Dados de Usuario
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime? Nascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Rua { get; set; }
        public int? NumeroCasa { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;

        // Dados de Medico
        public int IdEspecialidade { get; set; }
        public string Crm { get; set; } = string.Empty;
        public decimal? Honorario { get; set; }
    }
}