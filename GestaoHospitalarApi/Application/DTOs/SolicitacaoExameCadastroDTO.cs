namespace GestaoHospitalarApi.Application.DTOs
{
    public class SolicitacaoExameCadastroDto
    {
        public int IdProntuario { get; set; }
        public int IdExame { get; set; }
        public int IdMedico { get; set; }
        //public string? Justificativa { get; set; }
        public string Status { get; set; } = "SOLICITADO"; // SOLICITADO, EM_ANDAMENTO, REALIZADO, CANCELADO
        public string? Resultado { get; set; }
    }
}