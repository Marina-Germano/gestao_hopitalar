using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IAgendamentoService
    {
        Task CadastrarAsync(AgendamentoCadastroDto dto);
        Task<IEnumerable<AgendamentoListagemDto>> ObterTodosAsync();
    }
}