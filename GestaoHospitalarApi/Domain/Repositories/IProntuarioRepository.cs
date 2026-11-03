using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IProntuarioRepository : IGenericRepository<Prontuario>
    {
        Task<Prontuario?> ObterComDetalhesAsync(int idProntuario);
        Task<Prontuario?> ObterAtivoPorPacienteAsync(int idPaciente);
    }
}