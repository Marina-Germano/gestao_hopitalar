using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface ILeitoRepository : IGenericRepository<Leito>
    {
        Task<Leito?> ObterComAlaAsync(int id);
        Task<IEnumerable<Leito>> ObterTodosComAlaAsync();
        Task<IEnumerable<Leito>> ObterLeitosVagosAsync();
    }
}