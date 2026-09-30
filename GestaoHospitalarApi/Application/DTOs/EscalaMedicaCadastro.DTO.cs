namespace GestaoHospitalarApi.Application.DTOs.Escala
{
    public class EscalaMedicaCadastroDto
    {
        public string CrmMedico { get; set; } = string.Empty;

        public DateTime DataEscala { get; set; }

        public string HoraInicio { get; set; } = string.Empty;
        
        public string HoraFim { get; set; } = string.Empty;

        public int Plantao { get; set; } = 0;
    }
}