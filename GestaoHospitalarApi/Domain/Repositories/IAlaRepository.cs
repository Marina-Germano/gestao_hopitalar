using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IAlaRepository : IGenericRepository<Ala>
    {
        Task<IEnumerable<Ala>> ObterTodasOrdenadasAsync();
    }
}
