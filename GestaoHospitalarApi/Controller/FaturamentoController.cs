using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaturamentoController : ControllerBase
    {
        private readonly IFaturamentoService _service;
        public FaturamentoController(IFaturamentoService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var faturamentos = await _service.ObterTodosAsync();
            return Ok(ResultWrapper<IEnumerable<FaturamentoListagemDto>>.Ok(faturamentos));
        }

        [HttpPut("{id}/pagar")]
        public async Task<IActionResult> Pagar(int id)
        {
            await _service.PagarFaturamentoAsync(id);
            return Ok(ResultWrapper<object>.Ok(new object(), "Pagamento registrado com sucesso!"));
        }
    }
}