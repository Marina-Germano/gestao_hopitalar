using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IAgendamentoService
    {
        Task<IEnumerable<AgendamentoListagemDto>> ObterTodosAsync();
        Task<IEnumerable<AgendamentoListagemDto>> ObterPorMesAnoAsync(int ano, int mes);
        Task<IEnumerable<AgendamentoListagemDto>> ObterPorDataAsync(DateTime data);
        Task<AgendamentoListagemDto?> ObterPorIdAsync(int idAgendamento);
        Task<int> CadastrarAsync(AgendamentoCadastroDto dto);
        Task<bool> AtualizarStatusAsync(int idAgendamento, string novoStatus);
        Task<bool> CancelarAgendamentoAsync(int idAgendamento);
        Task<bool> ExcluirAgendamentoAsync(int idAgendamento);
    }
}