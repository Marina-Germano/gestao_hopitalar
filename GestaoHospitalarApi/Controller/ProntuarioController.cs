using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Wrappers;
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
            return Ok(ResultWrapper<IEnumerable<Prontuario>>.Ok(prontuarios, "Prontuários recuperados com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(id);
            if (prontuario == null)
                return NotFound(ResultWrapper<Prontuario>.Erro("Prontuário não encontrado."));

            return Ok(ResultWrapper<Prontuario>.Ok(prontuario, "Prontuário recuperado com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProntuarioCadastroDto dto)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(dto.IdPaciente);
            if (paciente == null)
                return NotFound(ResultWrapper<Paciente>.Erro("Paciente informado não existe."));

            var triagem = await _triagemRepository.GetByIdAsync(dto.IdTriagem);
            if (triagem == null)
                return NotFound(ResultWrapper<Triagem>.Erro("Triagem informada não existe."));

            var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);
            if (medico == null)
                return NotFound(ResultWrapper<Medico>.Erro("Médico informado não existe."));

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

            return StatusCode(201, ResultWrapper<object>.Ok(new { mensagem = "Prontuário aberto com sucesso", idProntuario = prontuario.IdProntuario }, "Prontuário aberto com sucesso"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProntuarioCadastroDto dto)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(id);
            if (prontuario == null)
                return NotFound(ResultWrapper<Prontuario>.Erro("Prontuário não encontrado."));

            prontuario.IdSala = dto.IdSala;
            //prontuario.RiscoEvasao = dto.RiscoEvasao.ToUpper();
            prontuario.Evolucao = dto.Evolucao;
            prontuario.StatusProntuario = dto.StatusProntuario.ToUpper();

            _prontuarioRepository.Update(prontuario);
            await _prontuarioRepository.SaveChangesAsync();

            return Ok(ResultWrapper<object>.Ok(new { mensagem = "Prontuário atualizado com sucesso." }, "Prontuário atualizado com sucesso."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(id);
            if (prontuario == null)
                return NotFound(ResultWrapper<Prontuario>.Erro("Prontuário não encontrado."));

            _prontuarioRepository.Delete(prontuario);
            await _prontuarioRepository.SaveChangesAsync();

            return Ok(ResultWrapper<string>.Ok(string.Empty, "Prontuário removido com sucesso."));
        }
    }
}