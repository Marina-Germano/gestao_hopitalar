using GestaoHospitalarApi.Models;
namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IMedicamentoRepository : IGenericRepository<Medicamento>
    {
        Task<IEnumerable<Medicamento>> ObterTodosComAlmoxarifadoAsync();
        Task<Medicamento?> ObterPorAlmoxarifadoIdAsync(int idAlmoxarifado);
    
    }
}
