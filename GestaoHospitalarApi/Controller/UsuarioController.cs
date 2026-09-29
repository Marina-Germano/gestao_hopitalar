using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;
using GestaoHospitalarApi.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(
            IGenericRepository<Usuario> usuarioRepository,
            IUsuarioService usuarioService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioRepository.GetAllAsync(
                usuario => usuario.IdPessoaNavigation
            );

            var usuariosDto = usuarios.Select(usuario => new UsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.IdPessoaNavigation?.Nome ?? string.Empty,
                Login = usuario.Login,
                Perfil = usuario.Perfil
            });

            return Ok(usuariosDto);
        }       

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(
                id,
                usuario => usuario.IdPessoaNavigation
            );

            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            var usuarioDto = new UsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.IdPessoaNavigation?.Nome ?? string.Empty,
                Login = usuario.Login,
                Perfil = usuario.Perfil
            };

            return Ok(usuarioDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] UsuarioCadastroDto dto)
        {
            var usuario = new Usuario
            {
                Login = dto.Login,
                Senha = dto.Senha,
                Perfil = dto.Perfil.ToUpper(),
                Ativo = 1,
                IdPessoaNavigation = new Pessoa
                {
                    Nome = dto.Nome,
                    Cpf = dto.Cpf,
                    Nascimento = dto.Nascimento.HasValue
                        ? DateOnly.FromDateTime(dto.Nascimento.Value)
                        : null,
                    Sexo = dto.Sexo,
                    Telefone = dto.Telefone,
                    Email = dto.Email,
                    Rua = dto.Rua,
                    NumeroCasa = dto.NumeroCasa,
                    Bairro = dto.Bairro,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    Cep = dto.Cep
                }
            };

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Usuário cadastrado com sucesso", idUsuario = usuario.IdUsuario });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioCadastroDto dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            usuario.Login = dto.Login;
            usuario.Senha = dto.Senha;
            usuario.Perfil = dto.Perfil.ToUpper();

            if (usuario.IdPessoaNavigation != null)
            {
                usuario.IdPessoaNavigation.Nome = dto.Nome;
                usuario.IdPessoaNavigation.Cpf = dto.Cpf;
                usuario.IdPessoaNavigation.Nascimento = dto.Nascimento.HasValue
                        ? DateOnly.FromDateTime(dto.Nascimento.Value)
                        : null;
                usuario.IdPessoaNavigation.Sexo = dto.Sexo;
                usuario.IdPessoaNavigation.Telefone = dto.Telefone;
                usuario.IdPessoaNavigation.Email = dto.Email;
                usuario.IdPessoaNavigation.Rua = dto.Rua;
                usuario.IdPessoaNavigation.NumeroCasa = dto.NumeroCasa;
                usuario.IdPessoaNavigation.Bairro = dto.Bairro;
                usuario.IdPessoaNavigation.Cidade = dto.Cidade;
                usuario.IdPessoaNavigation.Estado = dto.Estado;
                usuario.IdPessoaNavigation.Cep = dto.Cep;
            }

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Usuário atualizado com sucesso." });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            _usuarioRepository.Delete(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Usuário removido com sucesso." });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO dto)
        {
            var token = await _usuarioService.LoginAsync(dto);

            if (token == null)
                return Unauthorized(new { mensagem = "Login ou senha inválidos." });

            return Ok(new
            {
                token = token
            });
        }
    }
}