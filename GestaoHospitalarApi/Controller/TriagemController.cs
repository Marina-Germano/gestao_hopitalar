using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Bloqueia a rota inteira para usuários não logados
    public class TriagemController : ControllerBase
    {
        private readonly ITriagemService _triagemService;

        public TriagemController(ITriagemService triagemService)
        {
            _triagemService = triagemService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] TriagemCadastroDto dto)
        {
            try
            {
                // Extrai o login do usuário direto do Token JWT
                var loginUsuarioLogado = User.FindFirst(ClaimTypes.Name)?.Value 
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(loginUsuarioLogado))
                    return Unauthorized(ResultWrapper<object>.Erro("Usuário não identificado no token."));

                await _triagemService.CadastrarAsync(dto, loginUsuarioLogado);
                return Ok(ResultWrapper<object>.Ok(new object(), "Triagem registrada com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var triagens = await _triagemService.ObterTodasAsync();
            return Ok(ResultWrapper<IEnumerable<TriagemListagemDto>>.Ok(triagens));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var triagem = await _triagemService.ObterParaEdicaoAsync(id);
            if (triagem == null)
                return NotFound(ResultWrapper<object>.Erro("Triagem não encontrada."));

            return Ok(ResultWrapper<TriagemCadastroDto>.Ok(triagem));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] TriagemCadastroDto dto)
        {
            try
            {
                await _triagemService.AtualizarAsync(id, dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Triagem atualizada com sucesso!"));
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
                await _triagemService.ExcluirAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Triagem excluída com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }
    }
}