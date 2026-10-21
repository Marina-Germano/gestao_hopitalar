using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] >>>> se você quiser proteger a rota, descomente esta linha e configure o JWT
    public class LeitoController : ControllerBase
    {
        private readonly ILeitoService _leitoService;

        public LeitoController(ILeitoService leitoService)
        {
            _leitoService = leitoService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] LeitoCadastroDto dto)
        {
            try
            {
                await _leitoService.CadastrarAsync(dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Leito cadastrado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        // Rota principal para a tela do Admin/Médico: Agrupado por Alas com todos os status e cores
        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var leitos = await _leitoService.ObterTodosAsync();
            return Ok(ResultWrapper<IEnumerable<LeitoListagemDto>>.Ok(leitos));
        }

        // Rota para o dropdown da Triagem/Prontuário (Apenas Vagos)
        [HttpGet("vagos")]
        public async Task<IActionResult> ObterVagos()
        {
            var leitos = await _leitoService.ObterVagosAsync();
            return Ok(ResultWrapper<IEnumerable<LeitoListagemDto>>.Ok(leitos));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] LeitoCadastroDto dto)
        {
            try
            {
                await _leitoService.AtualizarAsync(id, dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Leito atualizado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        // AÇÃO DO ADMIN: Confirma que a higienização terminou
        [HttpPatch("{id}/liberar-higienizacao")]
        public async Task<IActionResult> LiberarHigienizacao(int id)
        {
            try
            {
                await _leitoService.LiberarHigienizacaoAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Leito liberado e marcado como Vago!"));
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
                await _leitoService.ExcluirAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Leito excluído com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }
    }
}