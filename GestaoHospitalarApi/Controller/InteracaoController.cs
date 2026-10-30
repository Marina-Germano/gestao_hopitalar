using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InteracaoController : ControllerBase
    {
        private readonly IInteracaoService _service;
        public InteracaoController(IInteracaoService service) => _service = service;

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] InteracaoCadastroDto dto)
        {
            await _service.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Interação medicamentosa registrada."));
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var interacoes = await _service.ObterTodasAsync();
            return Ok(ResultWrapper<IEnumerable<InteracaoListagemDto>>.Ok(interacoes));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var interacao = await _service.ObterPorIdAsync(id);
            return Ok(ResultWrapper<InteracaoListagemDto>.Ok(interacao));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] InteracaoCadastroDto dto)
        {
            await _service.AtualizarAsync(id, dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Interação atualizada com sucesso!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _service.ExcluirAsync(id);
            return Ok(ResultWrapper<object>.Ok(new object(), "Interação excluída."));
        }
    }
}