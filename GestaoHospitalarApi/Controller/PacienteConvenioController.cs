using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacienteConvenioController : ControllerBase
    {
        private readonly IPacienteConvenioService _service;
        public PacienteConvenioController(IPacienteConvenioService service) => _service = service;

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] PacienteConvenioCadastroDto dto)
        {
            await _service.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Convênio vinculado ao paciente com sucesso!"));
        }

        [HttpGet("paciente/{idPaciente}")]
        public async Task<IActionResult> ObterPorPaciente(int idPaciente)
        {
            var dados = await _service.ObterPorPacienteAsync(idPaciente);
            return Ok(ResultWrapper<IEnumerable<PacienteConvenioListagemDto>>.Ok(dados));
        }

        [HttpPut("{id}/inativar")]
        public async Task<IActionResult> Inativar(int id)
        {
            await _service.InativarAsync(id);
            return Ok(ResultWrapper<object>.Ok(new object(), "Vínculo com o convênio inativado!"));
        }
    }
}