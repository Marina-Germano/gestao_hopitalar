namespace GestaoHospitalarApi.Application.DTOs
{
    public class PacienteListagemDto
    {
        public int Id { get; set; } // Necessário para os botões de Editar, Arquivar e Excluir
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
    }
}