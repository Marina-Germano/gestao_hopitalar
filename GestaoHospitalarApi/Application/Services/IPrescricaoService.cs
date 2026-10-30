// using GestaoHospitalarApi.Application.DTOs;

// namespace GestaoHospitalarApi.Application.Services
// {
//     public interface IPrescricaoService
//     {
//         Task CadastrarAsync(PrescricaoCadastroDto dto);
//         Task<IEnumerable<PrescricaoListagemDto>> ObterPorProntuarioAsync(int idProntuario);
//     }
// }

using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IPrescricaoService
    {
        Task CadastrarAsync(PrescricaoCadastroDto dto);
        Task<IEnumerable<PrescricaoListagemDto>> ObterPorProntuarioAsync(int idProntuario);
        Task<PrescricaoListagemDto> ObterPorIdAsync(int id);
        Task AtualizarAsync(int id, PrescricaoCadastroDto dto);
        Task ExcluirAsync(int id);
    }
}