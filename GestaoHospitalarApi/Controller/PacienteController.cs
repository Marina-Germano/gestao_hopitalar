using Microsoft.AspNetCore.Mvc;
using GestaoHospitalarApi.Infra.EF; // Ajuste para o seu namespace
using GestaoHospitalarApi.DTOs;
using GestaoHospitalarApi.Models; // Ajuste para o seu namespace

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PacientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarPaciente([FromBody] PacienteCreateDTO dto)
        {
            // Inicia uma transação: se falhar o paciente, não salva o usuário pela metade
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Cria o Usuário base
                var usuario = new Usuario
                {
                    Nome = dto.Nome,
                    Cpf = dto.Cpf,
                    Nascimento = dto.Nascimento,
                    Sexo = dto.Sexo,
                    Telefone = dto.Telefone,
                    Email = dto.Email,
                    Rua = dto.Rua,
                    NumeroCasa = dto.NumeroCasa,
                    Bairro = dto.Bairro,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    Cep = dto.Cep,
                    Login = dto.Login,
                    Senha = dto.Senha, // Dica: No futuro, aplique um Hash aqui (ex: BCrypt)
                    Perfil = "PACIENTE",
                    Ativo = 1
                };

                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync(); // Salva para gerar o IdUsuario

                // 2. Cria o Paciente vinculado ao Usuário
                var paciente = new Paciente
                {
                    IdUsuario = usuario.IdUsuario,
                    Ativo = 1,
                    Alergias = dto.Alergias,
                    TipoSanguineo = dto.TipoSanguineo,
                    HistoricoClinico = dto.HistoricoClinico,
                    NomeResponsavel = dto.NomeResponsavel
                };

                _context.Paciente.Add(paciente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync(); // Confirma tudo no banco

                return StatusCode(201, new { mensagem = "Paciente cadastrado com sucesso", idPaciente = paciente.IdPaciente });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Desfaz tudo em caso de erro
                return BadRequest(new { mensagem = "Erro ao cadastrar paciente", erro = ex.Message });
            }
        }
    }
}