namespace GestaoHospitalarApi.Application.DTOs
{
    public class LeitoCadastroDto
    {
        public string Numero { get; set; } = string.Empty;
        public int IdAla { get; set; }
        public DateTime? DataHigienizacao { get; set; }
        public string? Situacao { get; set; } = "VAGO"; // VAGO, OCUPADO, HIGIENIZACAO
    }
}