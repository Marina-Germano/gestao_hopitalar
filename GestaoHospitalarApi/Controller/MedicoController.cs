using GestaoHospitalarApi.Application.DTOs; // Ou o namespace correto onde está o seu DTO
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Pode comentar para testar sem token por enquanto
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoService _medicoService;

        public MedicoController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] MedicoCadastroDto dto)
        {
            try
            {
                // Toda aquela cascata de Pessoa -> Usuario -> Especialidade -> Medico acontece aqui dentro!
                await _medicoService.CadastrarAsync(dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Médico cadastrado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        [HttpGet("ativos")]
        public async Task<IActionResult> ObterAtivos()
        {
            var medicos = await _medicoService.ObterTodosAtivosAsync();
            return Ok(ResultWrapper<object>.Ok(medicos));
        }

        [HttpGet("inativos")]
        public async Task<IActionResult> ObterInativos()
        {
            var medicos = await _medicoService.ObterTodosInativosAsync();
            return Ok(ResultWrapper<object>.Ok(medicos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var medico = await _medicoService.ObterParaEdicaoAsync(id);
            if (medico == null)
                return NotFound(ResultWrapper<object>.Erro("Médico não encontrado."));

            return Ok(ResultWrapper<MedicoCadastroDto>.Ok(medico));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] MedicoCadastroDto dto)
        {
            try
            {
                await _medicoService.AtualizarAsync(id, dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Médico atualizado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        [HttpPatch("{id}/arquivar")]
        public async Task<IActionResult> Arquivar(int id)
        {
            try
            {
                await _medicoService.ArquivarAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Médico arquivado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        [HttpPatch("{id}/ativar")]
        public async Task<IActionResult> Ativar(int id)
        {
            try
            {
                await _medicoService.AtivarAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Médico reativado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                await _medicoService.ExcluirAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Médico excluído com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }
    }
}