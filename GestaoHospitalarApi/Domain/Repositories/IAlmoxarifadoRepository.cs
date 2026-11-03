using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IAlmoxarifadoRepository : IGenericRepository<Almoxarifado>
    {
        Task<IEnumerable<Almoxarifado>> ObterTodosComDetalhesAsync();
    }
}