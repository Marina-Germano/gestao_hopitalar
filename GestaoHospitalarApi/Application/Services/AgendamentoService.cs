using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _repository;

        public AgendamentoService(IAgendamentoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AgendamentoListagemDto>> ObterTodosAsync()
        {
            var lista = await _repository.ObterTodosComDetalhesAsync();
            return lista.Select(MapearParaDto);
        }

        public async Task<IEnumerable<AgendamentoListagemDto>> ObterPorMesAnoAsync(int ano, int mes)
        {
            var lista = await _repository.ObterPorMesAnoAsync(ano, mes);
            return lista.Select(MapearParaDto);
        }

        public async Task<IEnumerable<AgendamentoListagemDto>> ObterPorDataAsync(DateTime data)
        {
            var lista = await _repository.ObterPorDataAsync(data);
            return lista.Select(MapearParaDto);
        }

        public async Task<AgendamentoListagemDto?> ObterPorIdAsync(int idAgendamento)
        {
            var agendamento = await _repository.ObterPorIdComDetalhesAsync(idAgendamento);
            return agendamento == null ? null : MapearParaDto(agendamento);
        }

        public async Task<int> CadastrarAsync(AgendamentoCadastroDto dto)
        {
            if (dto.IdPaciente <= 0 || dto.IdMedico <= 0 || !dto.IdSala.HasValue || dto.IdSala.Value <= 0)
                throw new ArgumentException("Paciente, Médico e Sala são obrigatórios.");

            var entidade = new Agendamento
            {
                IdPaciente = dto.IdPaciente,
                IdMedico = dto.IdMedico,
                IdSala = dto.IdSala.Value,
                DataHora = dto.DataHora,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "AGENDADO" : dto.Status.ToUpper()
            };

            await _repository.AddAsync(entidade);
            return entidade.IdAgendamento;
        }

        public Task<bool> AtualizarStatusAsync(int idAgendamento, string novoStatus) =>
            _repository.AtualizarStatusAsync(idAgendamento, novoStatus);

        public Task<bool> CancelarAgendamentoAsync(int idAgendamento) =>
            _repository.AtualizarStatusAsync(idAgendamento, "CANCELADO");

        public async Task<bool> ExcluirAgendamentoAsync(int idAgendamento)
        {
            var agendamento = await _repository.GetByIdAsync(idAgendamento);
            if (agendamento == null) return false;

            return await _repository.AtualizarStatusAsync(idAgendamento, "CANCELADO");
        }

        private static AgendamentoListagemDto MapearParaDto(Agendamento a)
        {
            return new AgendamentoListagemDto
            {
                IdAgendamento = a.IdAgendamento,
                NomePaciente = string.Empty,
                NomeMedico = string.Empty,
                NomeSala = string.Empty,
                DataHora = a.DataHora,
                Status = a.Status
            };
        }
    }
}