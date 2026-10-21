namespace GestaoHospitalarApi.Application.DTOs
{
    public class SalaCadastroDto
    {
        public string Nome { get; set; } = string.Empty;
        public int IdAla { get; set; } // Vinculo com a tabela Ala
    }

}