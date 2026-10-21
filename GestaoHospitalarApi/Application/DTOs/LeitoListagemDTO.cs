namespace GestaoHospitalarApi.Application.DTOs
{
    public class LeitoListagemDto
    {
        public int IdLeito { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Situacao { get; set; } = string.Empty; // "Livre", "Ocupado", "Manutenção"
        
        // Dados da Ala
        public int IdAla { get; set; }
        public string NomeAla { get; set; } = string.Empty; // Ex: "Cardiologia"
        public string AndarAla { get; set; } = string.Empty;
    }
}