using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IAlaService
    {
        Task CadastrarAsync(AlaCadastroDto dto);
        Task<IEnumerable<AlaListagemDto>> ObterTodasAsync();
        Task AtualizarAsync(int id, AlaCadastroDto dto);
        Task ExcluirAsync(int id);
    }
}