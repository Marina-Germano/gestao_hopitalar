using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface ISalaRepository : IGenericRepository<Sala>
    {
        Task<IEnumerable<Sala>> ObterTodasComAlaAsync();
        Task<IEnumerable<Sala>> ObterSalasLivresAsync(); // Para o dropdown da triagem
        Task<Sala?> ObterComAlaAsync(int id);
    }
}