using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface ILeitoService
    {
        Task CadastrarAsync(LeitoCadastroDto dto);
        Task AtualizarAsync(int id, LeitoCadastroDto dto);
        Task<IEnumerable<LeitoListagemDto>> ObterTodosAsync();
        Task<IEnumerable<LeitoListagemDto>> ObterVagosAsync();
        
        // Nova ação do Admin
        Task LiberarHigienizacaoAsync(int id);
        
        Task ExcluirAsync(int id);
    }
}