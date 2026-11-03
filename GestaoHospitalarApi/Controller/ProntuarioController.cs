using Microsoft.AspNetCore.Mvc;
using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProntuarioController : ControllerBase
    {
        private readonly IProntuarioService _service;

        public ProntuarioController(IProntuarioService service)
        {
            _service = service;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] ProntuarioCadastroDto dto)
        {
            try
            {
                var id = await _service.CadastrarAsync(dto);
                return Ok(new { sucesso = true, dados = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPost("encaminhar/{idTriagem}")]
        public async Task<IActionResult> Encaminhar(int idTriagem, [FromBody] TriagemEncaminhamentoDto dto)
        {
            try
            {
                var id = await _service.EncaminharTriagemAsync(idTriagem, dto);
                return Ok(new { sucesso = true, dados = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}