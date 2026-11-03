using GestaoHospitalarApi.Application.DTOs.Escala;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IEscalaMedicaService
    {
        Task<IEnumerable<EscalaMedicaListagemDto>> ObterPorMesAnoAsync(int ano, int mes);
        Task<IEnumerable<EscalaMedicaListagemDto>> ObterTodasAsync();
        Task<EscalaMedicaListagemDto?> ObterPorIdAsync(int idEscala);
        Task<int> CadastrarAsync(EscalaMedicaCadastroDto dto);
        Task<bool> AtualizarAsync(int idEscala, EscalaMedicaCadastroDto dto);
        Task<bool> ExcluirAsync(int idEscala);
    }
}