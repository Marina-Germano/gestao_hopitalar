using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface ITriagemService
    {
        Task CadastrarAsync(TriagemCadastroDto dto, string loginUsuarioLogado);
        Task<TriagemCadastroDto?> ObterParaEdicaoAsync(int id);
        Task AtualizarAsync(int id, TriagemCadastroDto dto);
        Task<IEnumerable<TriagemListagemDto>> ObterTodasAsync();
        Task ExcluirAsync(int id);
    }
}