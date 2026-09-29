using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IGenericRepository<Paciente> _pacienteRepository;
        private readonly IMemoryCache _cache;

        public PacientesController(IGenericRepository<Paciente> pacienteRepository,IMemoryCache cache)
        {
            _pacienteRepository = pacienteRepository;
            _cache = cache;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string cacheKey = "pacientes";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Paciente>? pacientesCache))
            {
                return Ok(pacientesCache);
            }

            var pacientes = await _pacienteRepository.GetAllAsync();

            _cache.Set(
                cacheKey,
                pacientes,
                TimeSpan.FromMinutes(5)
            );

            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);
            if (paciente == null)
                return NotFound(new { mensagem = "Paciente não encontrado." });

            return Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PacienteCadastroDto dto)
        {
            var paciente = new Paciente
            {
                Ativo = 1,
                Alergias = dto.Alergias,
                TipoSanguineo = dto.TipoSanguineo,
                HistoricoClinico = dto.HistoricoClinico,
                NomeResponsavel = dto.NomeResponsavel,
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

            await _pacienteRepository.AddAsync(paciente);
            await _pacienteRepository.SaveChangesAsync();

            _cache.Remove("pacientes");


            return StatusCode(201, new { mensagem = "Paciente cadastrado com sucesso", idPaciente = paciente.IdPaciente });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PacienteCadastroDto dto)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);
            if (paciente == null)
                return NotFound(new { mensagem = "Paciente não encontrado." });

            paciente.Alergias = dto.Alergias;
            paciente.TipoSanguineo = dto.TipoSanguineo;
            paciente.HistoricoClinico = dto.HistoricoClinico;
            paciente.NomeResponsavel = dto.NomeResponsavel;

            if (paciente.IdPessoaNavigation != null)
            {
                paciente.IdPessoaNavigation.Nome = dto.Nome;
                paciente.IdPessoaNavigation.Cpf = dto.Cpf;
                paciente.IdPessoaNavigation.Nascimento = dto.Nascimento.HasValue
                        ? DateOnly.FromDateTime(dto.Nascimento.Value)
                        : null;
                paciente.IdPessoaNavigation.Sexo = dto.Sexo;
                paciente.IdPessoaNavigation.Telefone = dto.Telefone;
                paciente.IdPessoaNavigation.Email = dto.Email;
                paciente.IdPessoaNavigation.Rua = dto.Rua;
                paciente.IdPessoaNavigation.NumeroCasa = dto.NumeroCasa;
                paciente.IdPessoaNavigation.Bairro = dto.Bairro;
                paciente.IdPessoaNavigation.Cidade = dto.Cidade;
                paciente.IdPessoaNavigation.Estado = dto.Estado;
                paciente.IdPessoaNavigation.Cep = dto.Cep;
            }

            _pacienteRepository.Update(paciente);
            await _pacienteRepository.SaveChangesAsync();

            _cache.Remove("pacientes");

            return Ok(new { mensagem = "Paciente atualizado com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);
            if (paciente == null)
                return NotFound(new { mensagem = "Paciente não encontrado." });

            _pacienteRepository.Delete(paciente);
            await _pacienteRepository.SaveChangesAsync();

            _cache.Remove("pacientes");

            return Ok(new { mensagem = "Paciente removido com sucesso." });
        }
    }
}