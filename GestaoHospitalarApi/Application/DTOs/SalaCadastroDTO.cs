namespace GestaoHospitalarApi.Application.DTOs.Sala
{
    public class SalaCadastroDto
    {
        public string Nome { get; set; } = string.Empty;

        public string? Andar { get; set; }
        public string NomeAla { get; set; } = string.Empty;
    }
}