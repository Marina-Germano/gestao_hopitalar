using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface ISalaService
    {
        Task CadastrarAsync(SalaCadastroDto dto);
        Task AtualizarAsync(int id, SalaCadastroDto dto);
        Task<IEnumerable<SalaListagemDto>> ObterTodasAsync();
        Task<IEnumerable<SalaListagemDto>> ObterLivresAsync(); // Útil para a tela de encaminhamento
        Task<SalaCadastroDto?> ObterParaEdicaoAsync(int id);
        Task ExcluirAsync(int id);
    }
}