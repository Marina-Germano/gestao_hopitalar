namespace GestaoHospitalarApi.Application.DTOs
{
    public class PrescricaoCadastroDto
    {
        public int IdProntuario { get; set; }
        public int IdMedico { get; set; }
        public int IdMedicamento { get; set; }
        public string? Dosagem { get; set; }
        public string? Aplicacao { get; set; } // O campo que estava faltando
        public string Horario { get; set; } = string.Empty; // O campo que estava faltando
        public string? Observacao { get; set; }
    }

        public class PrescricaoListagemDto : PrescricaoCadastroDto
    {
        public int IdPrescricao { get; set; }
        public string NomeMedico { get; set; } = string.Empty;
        public string NomeMedicamento { get; set; } = string.Empty;
    }
}