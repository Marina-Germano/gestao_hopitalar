using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspecialidadesController : ControllerBase
    {
        private readonly IGenericRepository<Especialidade> _repository;

        public EspecialidadesController(IGenericRepository<Especialidade> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var especialidades = await _repository.GetAllAsync();
            return Ok(especialidades);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var especialidade = await _repository.GetByIdAsync(id);
            if (especialidade == null)
                return NotFound(new { mensagem = "Especialidade não encontrada." });

            return Ok(especialidade);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EspecialidadeCadastroDto dto)
        {
            var especialidade = new Especialidade
            {
                DescricaoEspecialidade = dto.DescricaoEspecialidade
            };

            await _repository.AddAsync(especialidade);
            await _repository.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Especialidade cadastrada com sucesso", idEspecialidade = especialidade.IdEspecialidade });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EspecialidadeCadastroDto dto)
        {
            var especialidade = await _repository.GetByIdAsync(id);
            if (especialidade == null)
                return NotFound(new { mensagem = "Especialidade não encontrada." });

            especialidade.DescricaoEspecialidade = dto.DescricaoEspecialidade;

            _repository.Update(especialidade);
            await _repository.SaveChangesAsync();

            return Ok(new { mensagem = "Especialidade atualizada com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var especialidade = await _repository.GetByIdAsync(id);
            if (especialidade == null)
                return NotFound(new { mensagem = "Especialidade não encontrada." });

            _repository.Delete(especialidade);
            await _repository.SaveChangesAsync();

            return Ok(new { mensagem = "Especialidade removida com sucesso." });
        }
    }
}