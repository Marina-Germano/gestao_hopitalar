namespace GestaoHospitalarApi.Application.DTOs
{
    public class SolicitacaoExameLoteDto
    {
        public int IdProntuario { get; set; }
        public int IdMedico { get; set; }
        public List<int> IdsExames { get; set; } = new();
    }
}