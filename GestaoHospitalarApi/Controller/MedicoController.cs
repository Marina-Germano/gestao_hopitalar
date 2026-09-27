using Microsoft.AspNetCore.Mvc;
using GestaoHospitalarApi.Infra.EF;
using GestaoHospitalarApi.DTOs;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarMedico([FromBody] MedicoCreateDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var usuario = new Usuario
                {
                    Nome = dto.Nome,
                    Cpf = dto.Cpf,
                    Nascimento = dto.Nascimento,
                    Sexo = dto.Sexo,
                    Telefone = dto.Telefone,
                    Email = dto.Email,
                    Login = dto.Login,
                    Senha = dto.Senha,
                    Perfil = "MEDICO",
                    Ativo = 1
                };

                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();

                var medico = new Medico
                {
                    IdUsuario = usuario.IdUsuario,
                    IdEspecialidade = dto.IdEspecialidade,
                    Crm = dto.Crm,
                    Honorario = dto.Honorario
                };

                _context.Medico.Add(medico);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StatusCode(201, new { mensagem = "Médico cadastrado com sucesso", idMedico = medico.IdMedico });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { mensagem = "Erro ao cadastrar médico", erro = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}