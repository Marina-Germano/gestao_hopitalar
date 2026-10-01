namespace GestaoHospitalarApi.Application.DTOs.Medico
{
    public class MedicoListagemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Crm { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;
    }
}