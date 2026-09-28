namespace GestaoHospitalarApi.Application.DTOs
{
    public class MedicoCadastroDto : UsuarioCadastroDto
    {
        public int IdEspecialidade { get; set; }
        public string Crm { get; set; } = string.Empty;
        public decimal? Honorario { get; set; }
    }
}