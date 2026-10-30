using GestaoHospitalarApi.Application.DTOs;
namespace GestaoHospitalarApi.Application.Services
{
    public interface IInteracaoService
    {
        Task CadastrarAsync(InteracaoCadastroDto dto);
        Task<IEnumerable<InteracaoListagemDto>> ObterTodasAsync();
        Task<InteracaoListagemDto> ObterPorIdAsync(int id);
        Task AtualizarAsync(int id, InteracaoCadastroDto dto);
        Task ExcluirAsync(int id);
    }
}