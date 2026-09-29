using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> AddUsuarioAsync(UsuarioCadastroDto dto);

        Task<string?> LoginAsync(UsuarioLoginDTO dto);
    }
}