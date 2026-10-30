using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IPrescricaoRepository : IGenericRepository<Prescricao>
    {
        Task<IEnumerable<Prescricao>> ObterPorProntuarioAsync(int idProntuario);
    }
}