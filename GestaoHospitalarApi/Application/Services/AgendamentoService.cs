using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _agendamentoRepository;

        public AgendamentoService(IAgendamentoRepository agendamentoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
        }

        public async Task CadastrarAsync(AgendamentoCadastroDto dto)
        {
            var agendamento = new Agendamento
            {
                IdPaciente = dto.IdPaciente,
                IdMedico = dto.IdMedico,
                //IdSala = dto.IdSala,
                DataHora = dto.DataHora,
                Status = "AGENDADO"
            };

            await _agendamentoRepository.AddAsync(agendamento);
            await _agendamentoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AgendamentoListagemDto>> ObterTodosAsync()
        {
            var agendamentos = await _agendamentoRepository.ObterTodosComDetalhesAsync();
            return agendamentos.Select(a => new AgendamentoListagemDto
            {
                IdAgendamento = a.IdAgendamento,
                NomePaciente = a.IdPacienteNavigation?.IdPessoaNavigation?.Nome ?? "",
                NomeMedico = a.IdMedicoNavigation?.IdUsuarioNavigation?.IdPessoaNavigation?.Nome ?? "",
                NomeSala = a.IdSalaNavigation?.Nome ?? "",
                DataHora = a.DataHora,
                Status = a.Status ?? "AGENDADO"
            });
        }
    }
}