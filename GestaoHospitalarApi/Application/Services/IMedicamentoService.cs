using GestaoHospitalarApi.Application.DTOs;
namespace GestaoHospitalarApi.Application.Services
{
    public interface IMedicamentoService
    {
        Task CadastrarAsync(MedicamentoCadastroDto dto);
        Task AtualizarAsync(int id, MedicamentoCadastroDto dto);
        Task ExcluirAsync(int id);
        Task<IEnumerable<MedicamentoListagemDto>> ObterTodosAsync();
    }
}