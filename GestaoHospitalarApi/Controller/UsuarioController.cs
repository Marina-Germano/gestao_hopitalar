using Microsoft.AspNetCore.Mvc;
using GestaoHospitalarApi.Infra.EF; // Ajuste para o namespace do seu AppDbContext
using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Models; // Ajuste para o namespace das suas entidades

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarUsuario([FromBody] UsuarioCreateDTO dto)
        {
            try
            {
                // Converte o perfil para maiúsculo para evitar erro no CHECK do banco de dados
                var perfilValidado = dto.Perfil.Trim().ToUpper();

                var usuario = new Usuario
                {
                    Nome = dto.Nome,
                    Cpf = dto.Cpf,
                    Nascimento = dto.Nascimento,
                    Sexo = dto.Sexo?.ToUpper(), // Garante que fique em maiúsculo (MASCULINO, FEMININO, OUTRO)
                    Telefone = dto.Telefone,
                    Email = dto.Email,
                    Rua = dto.Rua,
                    NumeroCasa = dto.NumeroCasa,
                    Bairro = dto.Bairro,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    Cep = dto.Cep,
                    Login = dto.Login,
                    Senha = dto.Senha, // Importante: Considere criptografar a senha depois
                    Perfil = perfilValidado,
                    Ativo = 1 // 1 para Ativo por padrão
                };

                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();

                return StatusCode(201, new 
                { 
                    mensagem = "Usuário cadastrado com sucesso", 
                    idUsuario = usuario.IdUsuario,
                    perfil = usuario.Perfil
                });
            }
            catch (Exception ex)
            {
                // Um erro comum aqui será violação de UNIQUE no CPF, Email ou Login
                return BadRequest(new 
                { 
                    mensagem = "Erro ao cadastrar usuário. Verifique se o CPF, E-mail ou Login já estão em uso.", 
                    detalhe = ex.InnerException?.Message ?? ex.Message 
                });
            }
        }
    }
}