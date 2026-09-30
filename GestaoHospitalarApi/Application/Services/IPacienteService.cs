using GestaoHospitalarApi.Application.DTOs;

namespace GestaoHospitalarApi.Application.Services
{
    public interface IPacienteService
    {
        Task CadastrarAsync(PacienteCadastroDto dto);
        
        // Retorna o paciente com todos os dados preenchidos para a tela de edição
        Task<PacienteCadastroDto?> ObterParaEdicaoAsync(int id);
        
        // Recebe os dados alterados e salva no banco
        Task AtualizarAsync(int id, PacienteCadastroDto dto);
        
        // Listagens para a tela principal e lixeira
        Task<IEnumerable<PacienteListagemDto>> ObterTodosAtivosAsync();
        Task<IEnumerable<PacienteListagemDto>> ObterTodosInativosAsync();
        
        // Ações dos botões da lista
        Task ArquivarAsync(int id);
        Task AtivarAsync(int id);
        Task ExcluirAsync(int id);
    }
}