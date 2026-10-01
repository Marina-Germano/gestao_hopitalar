using GestaoHospitalarApi.Application.DTOs.Medico;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IMedicoService
    {
        Task CadastrarAsync(MedicoCadastroDto dto);
        Task<MedicoCadastroDto?> ObterParaEdicaoAsync(int id);
        Task AtualizarAsync(int id, MedicoCadastroDto dto);
        Task<IEnumerable<MedicoListagemDto>> ObterTodosAtivosAsync();
        Task<IEnumerable<MedicoListagemDto>> ObterTodosInativosAsync();
        Task ArquivarAsync(int id);
        Task AtivarAsync(int id);
        Task ExcluirAsync(int id);
    }
}