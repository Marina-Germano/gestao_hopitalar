using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IUsuarioService
    {
        // O método recebe um CreateDTO e devolve um UsuarioDTO
        Task<UsuarioDTO> AddUsuarioAsync(UsuarioCreateDTO dto);
    }
}