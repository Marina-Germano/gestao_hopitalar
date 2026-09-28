using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentosController : ControllerBase
    {
        private readonly IGenericRepository<Agendamento> _agendamentoRepository;
        private readonly IGenericRepository<Paciente> _pacienteRepository;
        private readonly IGenericRepository<Medico> _medicoRepository;

        public AgendamentosController(
            IGenericRepository<Agendamento> agendamentoRepository,
            IGenericRepository<Paciente> pacienteRepository,
            IGenericRepository<Medico> medicoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
            _pacienteRepository = pacienteRepository;
            _medicoRepository = medicoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var agendamentos = await _agendamentoRepository.GetAllAsync();
            return Ok(agendamentos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(id);
            if (agendamento == null)
                return NotFound(new { mensagem = "Agendamento não encontrado." });

            return Ok(agendamento);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AgendamentoCadastroDto dto)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(dto.IdPaciente);
            if (paciente == null)
                return NotFound(new { mensagem = "Paciente informado não existe." });

            var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);
            if (medico == null)
                return NotFound(new { mensagem = "Médico informado não existe." });

            var agendamento = new Agendamento
            {
                IdPaciente = dto.IdPaciente,
                IdMedico = dto.IdMedico,
                IdSala = dto.IdSala,
                DataHora = dto.DataHora,
                Status = string.IsNullOrEmpty(dto.Status) ? "AGENDADO" : dto.Status.ToUpper()
            };

            await _agendamentoRepository.AddAsync(agendamento);
            await _agendamentoRepository.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Agendamento criado com sucesso", idAgendamento = agendamento.IdAgendamento });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AgendamentoCadastroDto dto)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(id);
            if (agendamento == null)
                return NotFound(new { mensagem = "Agendamento não encontrado." });

            agendamento.IdPaciente = dto.IdPaciente;
            agendamento.IdMedico = dto.IdMedico;
            agendamento.IdSala = dto.IdSala;
            agendamento.DataHora = dto.DataHora;
            agendamento.Status = dto.Status.ToUpper();

            _agendamentoRepository.Update(agendamento);
            await _agendamentoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Agendamento atualizado com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(id);
            if (agendamento == null)
                return NotFound(new { mensagem = "Agendamento não encontrado." });

            _agendamentoRepository.Delete(agendamento);
            await _agendamentoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Agendamento removido com sucesso." });
        }
    }
}