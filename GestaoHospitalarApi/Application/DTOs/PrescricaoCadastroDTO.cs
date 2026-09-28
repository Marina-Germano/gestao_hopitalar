namespace GestaoHospitalarApi.Application.DTOs
{
    public class PrescricaoCadastroDto
    {
        public int IdProntuario { get; set; }
        public int IdMedico { get; set; }
        public int IdMedicamento { get; set; }
        public string Dosagem { get; set; } = string.Empty; // Ex: "1 comprimido de 8/8h por 7 dias"
        public string? Observacao { get; set; }
    }
}