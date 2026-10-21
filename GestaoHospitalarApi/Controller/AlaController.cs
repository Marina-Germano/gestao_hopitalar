using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using GestaoHospitalarApi.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlaController : ControllerBase
    {
        private readonly IAlaService _alaService;

        public AlaController(IAlaService alaService)
        {
            _alaService = alaService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] AlaCadastroDto dto)
        {
            await _alaService.CadastrarAsync(dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Ala cadastrada com sucesso!"));
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var alas = await _alaService.ObterTodasAsync();
            return Ok(ResultWrapper<IEnumerable<AlaListagemDto>>.Ok(alas));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AlaCadastroDto dto)
        {
            await _alaService.AtualizarAsync(id, dto);
            return Ok(ResultWrapper<object>.Ok(new object(), "Ala atualizada!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            await _alaService.ExcluirAsync(id);
            return Ok(ResultWrapper<object>.Ok(new object(), "Ala excluída!"));
        }
    }
}