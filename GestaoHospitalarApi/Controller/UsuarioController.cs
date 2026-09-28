using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;

        public UsuariosController(IGenericRepository<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            return Ok(usuario);
        }

        [HttpPost]
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
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            _usuarioRepository.Delete(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Usuário removido com sucesso." });
        }
    }
}