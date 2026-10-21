using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface ITriagemRepository : IGenericRepository<Triagem>
    {
        Task<Triagem?> ObterComDetalhesAsync(int id);
        Task<IEnumerable<Triagem>> ObterTodasComDetalhesAsync();
    }
}