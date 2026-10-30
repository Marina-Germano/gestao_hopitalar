using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AgendamentoController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] AgendamentoCadastroDto dto)
        {
            await _agendamentoService.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Agendamento criado com sucesso!"));
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var agendamentos = await _agendamentoService.ObterTodosAsync();
            return Ok(ResultWrapper<IEnumerable<AgendamentoListagemDto>>.Ok(agendamentos));
        }
    }
}