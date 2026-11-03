using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IAgendamentoRepository : IGenericRepository<Agendamento>
    {
        Task<IEnumerable<Agendamento>> ObterTodosComDetalhesAsync();
        Task<IEnumerable<Agendamento>> ObterPorMesAnoAsync(int ano, int mes);
        Task<IEnumerable<Agendamento>> ObterPorDataAsync(DateTime data);
        Task<Agendamento?> ObterPorIdComDetalhesAsync(int idAgendamento);
        Task<bool> AtualizarStatusAsync(int idAgendamento, string novoStatus);
    }
}