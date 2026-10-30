using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IInternacaoService
    {
        Task CadastrarAsync(InternacaoCadastroDto dto);
        Task<IEnumerable<InternacaoListagemDto>> ObterTodasAsync();
        Task DarAltaAsync(int idInternacao);
    }
}