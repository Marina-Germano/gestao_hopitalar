using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IPacienteConvenioService
    {
        Task CadastrarAsync(PacienteConvenioCadastroDto dto);
        Task<IEnumerable<PacienteConvenioListagemDto>> ObterPorPacienteAsync(int idPaciente);
        Task InativarAsync(int id);
    }
}