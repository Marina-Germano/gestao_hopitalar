using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Pode deixar comentado por enquanto para testar sem o token
    public class SalaController : ControllerBase
    {
        private readonly ISalaService _salaService;

        public SalaController(ISalaService salaService)
        {
            _salaService = salaService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] SalaCadastroDto dto)
        {
            try
            {
                await _salaService.CadastrarAsync(dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Sala cadastrada com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var salas = await _salaService.ObterTodasAsync();
            return Ok(ResultWrapper<IEnumerable<SalaListagemDto>>.Ok(salas));
        }

        [HttpGet("livres")]
        public async Task<IActionResult> ObterLivres()
        {
            // Rota especial para o dropdown do Encaminhamento da Triagem
            var salas = await _salaService.ObterLivresAsync();
            return Ok(ResultWrapper<IEnumerable<SalaListagemDto>>.Ok(salas));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var sala = await _salaService.ObterParaEdicaoAsync(id);
            if (sala == null)
                return NotFound(ResultWrapper<object>.Erro("Sala não encontrada."));

            return Ok(ResultWrapper<SalaCadastroDto>.Ok(sala));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalaCadastroDto dto)
        {
            try
            {
                await _salaService.AtualizarAsync(id, dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Sala atualizada com sucesso!"));
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
                await _salaService.ExcluirAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Sala excluída com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }
    }
}