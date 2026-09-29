using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Wrappers;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamesController : ControllerBase
    {
        private readonly IGenericRepository<Exame> _repository;

        public ExamesController(IGenericRepository<Exame> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exames = await _repository.GetAllAsync();
            return Ok(ResultWrapper<IEnumerable<Exame>>.Ok(exames, "Exames recuperados com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exame = await _repository.GetByIdAsync(id);
            if (exame == null)
                return NotFound(ResultWrapper<Exame>.Erro("Exame não encontrado."));

            return Ok(ResultWrapper<Exame>.Ok(exame, "Exame recuperado com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExameCadastroDto dto)
        {
            var exame = new Exame
            {
                Nome = dto.Nome,
                Valor = dto.Valor,
                Descricao = dto.Descricao
            };

            await _repository.AddAsync(exame);
            await _repository.SaveChangesAsync();

            return StatusCode(201, ResultWrapper<object>.Ok(new { mensagem = "Exame cadastrado com sucesso", idExame = exame.IdExame }, "Exame cadastrado com sucesso"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExameCadastroDto dto)
        {
            var exame = await _repository.GetByIdAsync(id);
            if (exame == null)
                return NotFound(ResultWrapper<Exame>.Erro("Exame não encontrado."));

            exame.Nome = dto.Nome;
            exame.Valor = dto.Valor;
            exame.Descricao = dto.Descricao;

            _repository.Update(exame);
            await _repository.SaveChangesAsync();

            return Ok(ResultWrapper<string>.Ok(string.Empty, "Exame atualizado com sucesso."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exame = await _repository.GetByIdAsync(id);
            if (exame == null)
                return NotFound(ResultWrapper<Exame>.Erro("Exame não encontrado."));

            _repository.Delete(exame);
            await _repository.SaveChangesAsync();

            return Ok(ResultWrapper<string>.Ok(string.Empty, "Exame removido com sucesso."));
        }
    }
}