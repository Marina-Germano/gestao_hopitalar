using Microsoft.AspNetCore.Mvc;
using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using System.Security.Claims;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class TriagemController : ControllerBase
    {
        private readonly ITriagemService _triagemService;

        public TriagemController(ITriagemService triagemService)
        {
            _triagemService = triagemService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] TriagemCadastroDto dto)
        {
            try
            {
                // Extrai o login do usuário logado (pode adaptar se o JWT não estiver configurado)
                var loginUsuarioLogado = User.FindFirst(ClaimTypes.Name)?.Value 
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "sistema_padrao";

                // 🟢 Recebe o ID que o Service agora retorna
                var idGerado = await _triagemService.CadastrarAsync(dto, loginUsuarioLogado);
                
                return Ok(new { sucesso = true, dados = idGerado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var triagens = await _triagemService.ObterTodasAsync();
            return Ok(new { sucesso = true, dados = triagens });
        }
    }
}