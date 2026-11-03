using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IAlmoxarifadoService
    {
        Task CadastrarAsync(AlmoxarifadoCadastroDto dto);
        Task<IEnumerable<AlmoxarifadoListagemDto>> ObterTodosAsync();
        Task<AlmoxarifadoListagemDto?> ObterPorIdAsync(int id);
        Task AtualizarAsync(int id, AlmoxarifadoCadastroDto dto);
        Task ExcluirAsync(int id);
    }
}