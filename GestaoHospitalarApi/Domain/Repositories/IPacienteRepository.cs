using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IPacienteRepository : IGenericRepository<Paciente>
    {
        Task<Paciente?> ObterComDetalhesAsync(int id);
        Task<IEnumerable<Paciente>> ObterTodosComPessoaAsync(int ativo);
    }
}