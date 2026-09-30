using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacienteController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        // POST: api/paciente/cadastrar
        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] PacienteCadastroDto dto)
        {
            try
            {
                await _pacienteService.CadastrarAsync(dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Paciente cadastrado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        // GET: api/paciente/ativos
        [HttpGet("ativos")]
        public async Task<IActionResult> ObterAtivos()
        {
            var pacientes = await _pacienteService.ObterTodosAtivosAsync();
            return Ok(ResultWrapper<IEnumerable<PacienteListagemDto>>.Ok(pacientes));
        }

        // GET: api/paciente/inativos
        [HttpGet("inativos")]
        public async Task<IActionResult> ObterInativos()
        {
            var pacientes = await _pacienteService.ObterTodosInativosAsync();
            return Ok(ResultWrapper<IEnumerable<PacienteListagemDto>>.Ok(pacientes));
        }

        // GET: api/paciente/5 (Para carregar o formulário de edição)
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var paciente = await _pacienteService.ObterParaEdicaoAsync(id);
            if (paciente == null)
                return NotFound(ResultWrapper<object>.Erro("Paciente não encontrado."));

            return Ok(paciente);
        }

        // PUT: api/paciente/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] PacienteCadastroDto dto)
        {
            try
            {
                await _pacienteService.AtualizarAsync(id, dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Paciente atualizado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        // PATCH: api/paciente/5/arquivar
        [HttpPatch("{id}/arquivar")]
        public async Task<IActionResult> Arquivar(int id)
        {
            try
            {
                await _pacienteService.ArquivarAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Paciente arquivado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        // DELETE: api/paciente/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                await _pacienteService.ExcluirAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Paciente excluído com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }
    }
}