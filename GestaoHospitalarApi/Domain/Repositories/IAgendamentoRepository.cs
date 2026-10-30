using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IAgendamentoRepository : IGenericRepository<Agendamento>
    {
        Task<IEnumerable<Agendamento>> ObterTodosComDetalhesAsync();
    }
}