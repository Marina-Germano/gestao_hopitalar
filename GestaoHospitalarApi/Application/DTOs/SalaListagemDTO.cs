namespace GestaoHospitalarApi.Application.DTOs
{
    public class SalaListagemDto
    {
        public int IdSala { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        
        // Dados da Ala
        public int IdAla { get; set; }
        public string NomeAla { get; set; } = string.Empty;
        //public string AndarAla { get; set; } = string.Empty;
    }
}