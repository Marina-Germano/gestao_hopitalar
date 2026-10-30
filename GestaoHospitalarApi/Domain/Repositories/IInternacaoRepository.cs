using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IInternacaoRepository : IGenericRepository<Internacao>
    {
        Task<IEnumerable<Internacao>> ObterTodasComDetalhesAsync();

        Task<Internacao?> ObterComDetalhesFaturamentoAsync(int idInternacao);
    }
}