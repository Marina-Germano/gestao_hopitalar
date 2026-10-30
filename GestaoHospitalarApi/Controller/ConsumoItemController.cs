using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumoItemController : ControllerBase
    {
        private readonly IConsumoItemService _service;
        public ConsumoItemController(IConsumoItemService service) => _service = service;

        [HttpPost("registrar")]
        public async Task<IActionResult> Cadastrar([FromBody] ConsumoItemCadastroDto dto)
        {
            await _service.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Consumo registrado e estoque atualizado."));
        }

        [HttpGet("internacao/{idInternacao}")]
        public async Task<IActionResult> ObterPorInternacao(int idInternacao)
        {
            var consumos = await _service.ObterPorInternacaoAsync(idInternacao);
            return Ok(ResultWrapper<IEnumerable<ConsumoItemListagemDto>>.Ok(consumos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var consumo = await _service.ObterPorIdAsync(id);
            return Ok(ResultWrapper<ConsumoItemListagemDto>.Ok(consumo));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ConsumoItemCadastroDto dto)
        {
            await _service.AtualizarAsync(id, dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Consumo atualizado e estoque corrigido!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _service.ExcluirAsync(id);
            return Ok(ResultWrapper<object>.Ok(new object(), "Consumo excluído. O item voltou para o estoque."));
        }

    }
}