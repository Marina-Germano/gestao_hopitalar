using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> GetAllUsuariosAsync();
        Task<UsuarioDTO> GetUsuarioByIdAsync(int id);
        Task<UsuarioDTO> AddUsuarioAsync(UsuarioDTO usuarioDto);
        Task UpdateUsuarioAsync(UsuarioDTO usuarioDto);
        Task DeleteUsuarioAsync(int id);
    }
}