using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InternacaoController : ControllerBase
    {
        private readonly IInternacaoService _service;

        public InternacaoController(IInternacaoService service)
        {
            _service = service;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] InternacaoCadastroDto dto)
        {
            await _service.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Internação registrada e leito ocupado!"));
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var dados = await _service.ObterTodasAsync();
            return Ok(ResultWrapper<IEnumerable<InternacaoListagemDto>>.Ok(dados));
        }

        [HttpPost("alta/{id}")]
        public async Task<IActionResult> DarAlta(int id)
        {
            await _service.DarAltaAsync(id);
            return Ok(ResultWrapper<object>.Ok(new object(), "Alta realizada com sucesso. Leito enviado para higienização e faturamento gerado!"));
        }
    }
}