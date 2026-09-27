namespace GestaoHospitalarApi.Application.DTOs
{
    public class UsuarioCreateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime? Nascimento { get; set; }
        
        // MASCULINO, FEMININO, OUTRO
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
        
        // ADMIN, ENFERMEIRO, RECEPCAO, FINANCEIRO (Medico e Paciente tem controllers proprios)
        public string Perfil { get; set; } = string.Empty; 
    }
}