namespace GestaoHospitalarApi.Application.DTOs
{
    public class InternacaoCadastroDto
    {
        public int IdProntuario { get; set; }
        public int IdLeito { get; set; }
        public string Isolamento { get; set; } = "NAO"; // SIM ou NAO
        public string StatusInternacao { get; set; } = "ATIVA"; // ATIVA, ALTA, TRANSFERIDO

    }
}