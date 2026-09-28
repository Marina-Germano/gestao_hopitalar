namespace GestaoHospitalarApi.Application.DTOs
{
    public class PacienteCadastroDto : PessoaCadastroDto
    {
        public string? Alergias { get; set; }
        public string? TipoSanguineo { get; set; }
        public string? HistoricoClinico { get; set; }
        public string? NomeResponsavel { get; set; }
    }
}