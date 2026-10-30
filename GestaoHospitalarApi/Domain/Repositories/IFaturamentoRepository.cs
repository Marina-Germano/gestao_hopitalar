using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IFaturamentoRepository : IGenericRepository<Faturamento>
    {
        Task<IEnumerable<Faturamento>> ObterTodosComDetalhesAsync();
        Task<Faturamento?> ObterPorInternacaoAsync(int idInternacao);
    }
}