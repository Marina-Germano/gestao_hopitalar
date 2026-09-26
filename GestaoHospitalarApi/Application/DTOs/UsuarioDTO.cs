namespace GestaoHospitalarApi.Application.DTOs
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public int Ativo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty; // Em produção, nunca retorne a senha!
        public string? Email { get; set; }
        public string Perfil { get; set; } = string.Empty;
    }
}