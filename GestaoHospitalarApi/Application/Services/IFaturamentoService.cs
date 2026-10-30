using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IFaturamentoService
    {
        Task GerarFaturamentoAltaAsync(int idInternacao);
        Task<IEnumerable<FaturamentoListagemDto>> ObterTodosAsync();
        Task PagarFaturamentoAsync(int idFaturamento);
    }
}