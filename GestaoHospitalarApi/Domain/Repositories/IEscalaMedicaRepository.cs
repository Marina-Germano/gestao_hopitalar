using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IEscalaMedicaRepository : IGenericRepository<EscalaMedica>
    {
        Task<IEnumerable<EscalaMedica>> ObterPorMesAnoAsync(int ano, int mes);
        Task<IEnumerable<EscalaMedica>> ObterTodasComMedicoAsync();
        Task<EscalaMedica?> ObterPorIdComMedicoAsync(int idEscala);
        Task<int> CadastrarAsync(EscalaMedica escala);
        Task<bool> AtualizarAsync(EscalaMedica escala);
        Task<bool> ExcluirAsync(int idEscala);
    }
}