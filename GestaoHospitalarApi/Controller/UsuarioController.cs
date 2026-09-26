using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
                return NotFound(new { message = "Usuário não encontrado" });

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioDTO usuarioDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _usuarioService.AddUsuarioAsync(usuarioDto);
            return CreatedAtAction(nameof(GetById), new { id = created.IdUsuario }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioDTO usuarioDto)
        {
            if (id != usuarioDto.IdUsuario)
                return BadRequest(new { message = "O ID da URL não corresponde ao ID do corpo da requisição." });

            try
            {
                await _usuarioService.UpdateUsuarioAsync(usuarioDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _usuarioService.DeleteUsuarioAsync(id);
            return NoContent();
        }
    }
}