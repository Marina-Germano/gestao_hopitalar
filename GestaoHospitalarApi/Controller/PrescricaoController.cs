using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescricaoController : ControllerBase
    {
        private readonly IPrescricaoService _service;

        public PrescricaoController(IPrescricaoService service) => _service = service;

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] PrescricaoCadastroDto dto)
        {
            await _service.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Prescrição adicionada ao prontuário."));
        }

        [HttpGet("prontuario/{idProntuario}")]
        public async Task<IActionResult> ObterPorProntuario(int idProntuario)
        {
            var prescricoes = await _service.ObterPorProntuarioAsync(idProntuario);
            return Ok(ResultWrapper<IEnumerable<PrescricaoListagemDto>>.Ok(prescricoes));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var prescricao = await _service.ObterPorIdAsync(id);
            return Ok(ResultWrapper<PrescricaoListagemDto>.Ok(prescricao));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] PrescricaoCadastroDto dto)
        {
            await _service.AtualizarAsync(id, dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Prescrição atualizada com sucesso!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _service.ExcluirAsync(id);
            return Ok(ResultWrapper<object>.Ok(new object(), "Prescrição excluída."));
        }
    }
}