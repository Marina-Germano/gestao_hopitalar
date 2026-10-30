namespace GestaoHospitalarApi.Application.DTOs
{
    public class InternacaoCadastroDto
    {
        public int IdProntuario { get; set; }
        public int IdLeito { get; set; }
        public string Isolamento { get; set; } = "NAO"; // SIM ou NAO
        public string StatusInternacao { get; set; } = "ATIVA"; // ATIVA, ALTA, TRANSFERIDO

    }

    public class InternacaoListagemDto
    {
        public int IdInternacao { get; set; }
        public string NomePaciente { get; set; } = string.Empty;
        public string NumeroLeito { get; set; } = string.Empty;
        public DateTime? DataEntrada { get; set; }
        public DateTime? DataAlta { get; set; }
        public string StatusInternacao { get; set; } = string.Empty;
    }

}