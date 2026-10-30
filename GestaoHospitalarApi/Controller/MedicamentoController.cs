using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentoController : ControllerBase
    {
        private readonly IMedicamentoService _service;
        public MedicamentoController(IMedicamentoService service) => _service = service;

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] MedicamentoCadastroDto dto)
        {
            await _service.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Medicamento cadastrado no estoque e na clínica!"));
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var meds = await _service.ObterTodosAsync();
            return Ok(ResultWrapper<IEnumerable<MedicamentoListagemDto>>.Ok(meds));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] MedicamentoCadastroDto dto)
        {
            await _service.AtualizarAsync(id, dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Medicamento atualizado!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _service.ExcluirAsync(id);
            return Ok(ResultWrapper<object>.Ok(new object(), "Medicamento excluído!"));
        }
    }
}