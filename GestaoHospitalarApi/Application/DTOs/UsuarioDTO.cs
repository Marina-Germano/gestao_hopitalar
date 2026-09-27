namespace GestaoHospitalarApi.Application.DTOs
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;
    }
}