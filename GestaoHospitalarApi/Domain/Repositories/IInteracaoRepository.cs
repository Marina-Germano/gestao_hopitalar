using GestaoHospitalarApi.Models;
namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IInteracaoRepository : IGenericRepository<InteracaoMedicamentosa>
    {
        Task<IEnumerable<InteracaoMedicamentosa>> ObterTodasComDetalhesAsync();
    }
}
