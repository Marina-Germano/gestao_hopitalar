using GestaoHospitalarApi.Application.DTOs; // Ajuste para o namespace exato do seu DTO de convênio

namespace GestaoHospitalarApi.Application.Services
{
    public interface IConvenioService
    {
        Task CadastrarAsync(ConvenioCadastroDto dto);
        Task<IEnumerable<ConvenioCadastroDto>> ObterTodosAsync();
        Task ExcluirAsync(int id);
    }
}