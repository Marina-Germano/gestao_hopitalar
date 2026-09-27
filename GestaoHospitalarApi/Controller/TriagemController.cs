using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;
using GestaoHospitalarApi.DTOs;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TriagensController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TriagensController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarTriagem([FromBody] TriagemCreateDTO dto)
        {
            // Verifica se o paciente realmente existe
            var pacienteExiste = await _context.Paciente.AnyAsync(p => p.IdPaciente == dto.IdPaciente);
            if (!pacienteExiste)
            {
                return NotFound(new { mensagem = "Paciente não encontrado no sistema." });
            }

            var triagem = new Triagem
            {
                IdPaciente = dto.IdPaciente,
                ResponsavelTriagem = dto.ResponsavelTriagem,
                Pressao = dto.Pressao,
                Temperatura = dto.Temperatura,
                FrequenciaCardiaca = dto.FrequenciaCardiaca,
                Saturacao = dto.Saturacao,
                EscalaDor = dto.EscalaDor,
                Risco = dto.Risco,
                Queixa = dto.Queixa,
                Alergias = dto.Alergias,
                Observacoes = dto.Observacoes,
                Internacao = dto.Internacao
            };

            _context.Triagem.Add(triagem);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Triagem registrada com sucesso", idTriagem = triagem.IdTriagem });
        }
    }
}