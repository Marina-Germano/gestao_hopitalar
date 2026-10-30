using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitacaoExameController : ControllerBase
    {
        private readonly ISolicitacaoExameService _service;

        public SolicitacaoExameController(ISolicitacaoExameService service) => _service = service;

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] SolicitacaoExameCadastroDto dto)
        {
            await _service.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Exame solicitado."));
        }

        [HttpGet("prontuario/{idProntuario}")]
        public async Task<IActionResult> ObterPorProntuario(int idProntuario)
        {
            var solicitacoes = await _service.ObterPorProntuarioAsync(idProntuario);
            return Ok(ResultWrapper<IEnumerable<SolicitacaoExameListagemDto>>.Ok(solicitacoes));
        }

        [HttpPut("{id}/resultado")]
        public async Task<IActionResult> LancarResultado(int id, [FromBody] string resultado)
        {
            await _service.LancarResultadoAsync(id, resultado);
            return Ok(ResultWrapper<object>.Ok(new object(), "Resultado do exame lançado com sucesso."));
        }
    }
}