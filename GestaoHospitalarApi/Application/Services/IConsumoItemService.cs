using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IConsumoItemService
    {
        Task CadastrarAsync(ConsumoItemCadastroDto dto);
        Task<IEnumerable<ConsumoItemListagemDto>> ObterPorInternacaoAsync(int idInternacao);
        Task<ConsumoItemListagemDto> ObterPorIdAsync(int id);
        Task AtualizarAsync(int id, ConsumoItemCadastroDto dto);
        Task ExcluirAsync(int id);
    }
}