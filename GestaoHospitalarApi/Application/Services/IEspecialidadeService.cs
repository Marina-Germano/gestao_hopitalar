using GestaoHospitalarApi.Application.DTOs;
namespace GestaoHospitalarApi.Application.Services
{
    public interface IEspecialidadeService
    {
        Task CadastrarAsync(EspecialidadeCadastroDto dto);
        Task<IEnumerable<EspecialidadeCadastroDto>> ObterTodosAsync();
        Task ExcluirAsync(int id);
    }
}