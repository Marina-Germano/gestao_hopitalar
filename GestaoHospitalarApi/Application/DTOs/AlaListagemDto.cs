namespace GestaoHospitalarApi.Application.DTOs
{
    public class AlaListagemDto
    {
        public int IdAla { get; set; }
        public string NomeAla { get; set; } = string.Empty;
        public string Andar { get; set; } = string.Empty;
    }
}