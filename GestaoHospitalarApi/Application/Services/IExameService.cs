using GestaoHospitalarApi.Application.DTOs;
namespace GestaoHospitalarApi.Application.Services
{
    public interface IExameService
    {
        Task CadastrarAsync(ExameCadastroDto dto);
        Task AtualizarAsync(int id, ExameCadastroDto dto);
        Task ExcluirAsync(int id);
        Task<IEnumerable<ExameListagemDto>> ObterTodosAsync();
    }
}