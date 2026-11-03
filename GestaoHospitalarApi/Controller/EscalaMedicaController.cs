using Microsoft.AspNetCore.Mvc;
using GestaoHospitalarApi.Application.DTOs.Escala;
using GestaoHospitalarApi.Application.Services;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EscalaMedicaController : ControllerBase
    {
        private readonly IEscalaMedicaService _service;

        public EscalaMedicaController(IEscalaMedicaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var resultado = await _service.ObterTodasAsync();
            return Ok(new { sucesso = true, dados = resultado });
        }

        [HttpGet("mes/{ano}/{mes}")]
        public async Task<IActionResult> ObterPorMes(int ano, int mes)
        {
            var resultado = await _service.ObterPorMesAnoAsync(ano, mes);
            return Ok(new { sucesso = true, dados = resultado });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var escala = await _service.ObterPorIdAsync(id);
            if (escala == null)
                return NotFound(new { sucesso = false, mensagem = "Escala médica não encontrada." });

            return Ok(new { sucesso = true, dados = escala });
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Criar([FromBody] EscalaMedicaCadastroDto dto)
        {
            try
            {
                var id = await _service.CadastrarAsync(dto);
                return Ok(new { sucesso = true, mensagem = "Escala médica cadastrada com sucesso.", dados = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] EscalaMedicaCadastroDto dto)
        {
            var ok = await _service.AtualizarAsync(id, dto);
            if (!ok)
                return NotFound(new { sucesso = false, mensagem = "Escala médica não encontrada." });

            return Ok(new { sucesso = true, mensagem = "Escala médica atualizada com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var ok = await _service.ExcluirAsync(id);
            if (!ok)
                return NotFound(new { sucesso = false, mensagem = "Escala médica não encontrada." });

            return Ok(new { sucesso = true, mensagem = "Escala médica removida com sucesso." });
        }
    }
}