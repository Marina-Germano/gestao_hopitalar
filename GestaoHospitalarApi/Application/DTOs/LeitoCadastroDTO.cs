namespace GestaoHospitalarApi.Application.DTOs
{
    public class LeitoCadastroDto
    {
        public string Numero { get; set; } = string.Empty;
        public string? Ala { get; set; }
        public string? Andar { get; set; }
        public DateTime? DataHigienizacao { get; set; }
        public string? Situacao { get; set; } = "VAGO"; // VAGO, OCUPADO, HIGIENIZACAO
    }
}