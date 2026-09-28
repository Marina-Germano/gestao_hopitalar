namespace GestaoHospitalarApi.Application.DTOs
{
    public class PessoaCadastroDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Cpf { get; set; }
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
    }
}