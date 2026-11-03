using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlmoxarifadoController : ControllerBase
    {
        private readonly IAlmoxarifadoService _almoxarifadoService;

        public AlmoxarifadoController(IAlmoxarifadoService almoxarifadoService)
        {
            _almoxarifadoService = almoxarifadoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var itens = await _almoxarifadoService.ObterTodosAsync();
            return Ok(ResultWrapper<IEnumerable<AlmoxarifadoListagemDto>>.Ok(itens));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var item = await _almoxarifadoService.ObterPorIdAsync(id);
            if (item == null)
                return NotFound(ResultWrapper<object>.Erro("Item não encontrado."));

            return Ok(ResultWrapper<AlmoxarifadoListagemDto>.Ok(item));
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] AlmoxarifadoCadastroDto dto)
        {
            try
            {
                await _almoxarifadoService.CadastrarAsync(dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Item cadastrado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AlmoxarifadoCadastroDto dto)
        {
            try
            {
                await _almoxarifadoService.AtualizarAsync(id, dto);
                return Ok(ResultWrapper<object>.Ok(new object(), "Item atualizado com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                await _almoxarifadoService.ExcluirAsync(id);
                return Ok(ResultWrapper<object>.Ok(new object(), "Item excluído com sucesso!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ResultWrapper<object>.Erro(ex.Message));
            }
        }
    }
}