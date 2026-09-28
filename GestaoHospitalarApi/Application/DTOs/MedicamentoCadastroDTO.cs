namespace GestaoHospitalarApi.Application.DTOs
{
    public class MedicamentoCadastroDto
    {
        public int IdAlmoxarifado { get; set; }
        public string? PrincipioAtivo { get; set; }
        public string? Contraindicacoes { get; set; }
    }
}