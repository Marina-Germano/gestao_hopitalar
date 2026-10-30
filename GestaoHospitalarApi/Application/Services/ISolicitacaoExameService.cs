using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface ISolicitacaoExameService
    {
        Task CadastrarAsync(SolicitacaoExameCadastroDto dto);
        Task<IEnumerable<SolicitacaoExameListagemDto>> ObterPorProntuarioAsync(int idProntuario);
        Task LancarResultadoAsync(int idSolicitacao, string resultado);
    }
}