using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IMedicoRepository : IGenericRepository<Medico>
    {
        Task<Medico?> ObterComDetalhesAsync(int id);
        Task<IEnumerable<Medico>> ObterTodosComPessoaEEspecialidadeAsync(int ativo);
    }
}