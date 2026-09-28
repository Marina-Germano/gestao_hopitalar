namespace GestaoHospitalarApi.Application.DTOs
{
    public class UsuarioCadastroDto : PessoaCadastroDto
    {
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty; // ADMIN, ENFERMEIRO, RECEPCAO, FINANCEIRO
    }
}