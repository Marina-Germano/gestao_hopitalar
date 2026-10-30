using GestaoHospitalarApi.Models;
namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IConsumoItemRepository : IGenericRepository<ConsumoItem>
    {
        Task<IEnumerable<ConsumoItem>> ObterPorInternacaoAsync(int idInternacao);
    }
}