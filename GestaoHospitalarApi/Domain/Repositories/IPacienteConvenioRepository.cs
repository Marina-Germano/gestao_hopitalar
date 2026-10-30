using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IPacienteConvenioRepository : IGenericRepository<PacienteConvenio>
    {
        Task<IEnumerable<PacienteConvenio>> ObterPorPacienteAsync(int idPaciente);
    }
}