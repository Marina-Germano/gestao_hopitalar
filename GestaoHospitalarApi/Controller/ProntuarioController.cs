using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProntuariosController : ControllerBase
    {
        private readonly IGenericRepository<Prontuario> _prontuarioRepository;
        private readonly IGenericRepository<Paciente> _pacienteRepository;
        private readonly IGenericRepository<Triagem> _triagemRepository;
        private readonly IGenericRepository<Medico> _medicoRepository;

        public ProntuariosController(
            IGenericRepository<Prontuario> prontuarioRepository,
            IGenericRepository<Paciente> pacienteRepository,
            IGenericRepository<Triagem> triagemRepository,
            IGenericRepository<Medico> medicoRepository)
        {
            _prontuarioRepository = prontuarioRepository;
            _pacienteRepository = pacienteRepository;
            _triagemRepository = triagemRepository;
            _medicoRepository = medicoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var prontuarios = await _prontuarioRepository.GetAllAsync();
            return Ok(prontuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(id);
            if (prontuario == null)
                return NotFound(new { mensagem = "Prontuário não encontrado." });

            return Ok(prontuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProntuarioCadastroDto dto)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(dto.IdPaciente);
            if (paciente == null)
                return NotFound(new { mensagem = "Paciente informado não existe." });

            var triagem = await _triagemRepository.GetByIdAsync(dto.IdTriagem);
            if (triagem == null)
                return NotFound(new { mensagem = "Triagem informada não existe." });

            var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);
            if (medico == null)
                return NotFound(new { mensagem = "Médico informado não existe." });

            var prontuario = new Prontuario
            {
                IdPaciente = dto.IdPaciente,
                IdTriagem = dto.IdTriagem,
                IdMedico = dto.IdMedico,
                IdSala = dto.IdSala,
                RiscoEvasao = dto.RiscoEvasao.ToUpper(),
                Evolucao = dto.Evolucao,
                DataAbertura = DateTime.Now,
                StatusProntuario = string.IsNullOrEmpty(dto.StatusProntuario) ? "ATIVO" : dto.StatusProntuario.ToUpper()
            };

            await _prontuarioRepository.AddAsync(prontuario);
            await _prontuarioRepository.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Prontuário aberto com sucesso", idProntuario = prontuario.IdProntuario });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProntuarioCadastroDto dto)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(id);
            if (prontuario == null)
                return NotFound(new { mensagem = "Prontuário não encontrado." });

            prontuario.IdSala = dto.IdSala;
            //prontuario.RiscoEvasao = dto.RiscoEvasao.ToUpper();
            prontuario.Evolucao = dto.Evolucao;
            prontuario.StatusProntuario = dto.StatusProntuario.ToUpper();

            _prontuarioRepository.Update(prontuario);
            await _prontuarioRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Prontuário atualizado com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(id);
            if (prontuario == null)
                return NotFound(new { mensagem = "Prontuário não encontrado." });

            _prontuarioRepository.Delete(prontuario);
            await _prontuarioRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Prontuário removido com sucesso." });
        }
    }
}