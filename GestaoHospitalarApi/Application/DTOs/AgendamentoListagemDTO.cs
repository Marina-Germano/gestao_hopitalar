namespace GestaoHospitalarApi.Application.DTOs
{
    public class AgendamentoListagemDto
    {
        public int IdAgendamento { get; set; }
        public string NomePaciente { get; set; } = string.Empty;
        public string NomeMedico { get; set; } = string.Empty;
        public string NomeSala { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}