namespace GestaoHospitalarApi.Application.DTOs
{
    public class ExameCadastroDto
    {
        public string Nome { get; set; } = string.Empty;
        public decimal? Valor { get; set; }
        public string? Descricao { get; set; }
    }
}