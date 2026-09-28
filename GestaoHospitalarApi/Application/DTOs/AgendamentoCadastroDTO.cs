namespace GestaoHospitalarApi.Application.DTOs
{
    public class AgendamentoCadastroDto
    {
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public int? IdSala { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; } = "AGENDADO"; // AGENDADO, CONFIRMADO, CANCELADO, FINALIZADO
    }
}