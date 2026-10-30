using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface ISolicitacaoExameRepository : IGenericRepository<SolicitacaoExame>
    {
        Task<IEnumerable<SolicitacaoExame>> ObterPorProntuarioAsync(int idProntuario);
    }
}