using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Wrappers;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConveniosController : ControllerBase
    {
        private readonly IGenericRepository<Convenio> _repository;

        public ConveniosController(IGenericRepository<Convenio> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var convenios = await _repository.GetAllAsync();
            return Ok(ResultWrapper<IEnumerable<Convenio>>.Ok(convenios, "Convênios recuperados com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var convenio = await _repository.GetByIdAsync(id);
            if (convenio == null)
                return NotFound(ResultWrapper<Convenio>.Erro("Convênio não encontrado."));

            return Ok(ResultWrapper<Convenio>.Ok(convenio, "Convênio recuperado com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConvenioCadastroDto dto)
        {
            var convenio = new Convenio
            {
                NomeConvenio = dto.NomeConvenio,
                TipoLeito = dto.TipoLeito,
                CobreInternacao = dto.CobreInternacao,
                CobreExames = dto.CobreExames,
                CobreCirurgia = dto.CobreCirurgia,
                LimiteMedicamento = dto.LimiteMedicamento,
                PercentualCobertura = dto.PercentualCobertura,
                Ativo = dto.Ativo
            };

            await _repository.AddAsync(convenio);
            await _repository.SaveChangesAsync();

            return StatusCode(201, ResultWrapper<object>.Ok(new { mensagem = "Convênio cadastrado com sucesso", idConvenio = convenio.IdConvenio }, "Convênio cadastrado com sucesso"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ConvenioCadastroDto dto)
        {
            var convenio = await _repository.GetByIdAsync(id);
            if (convenio == null)
                return NotFound(ResultWrapper<Convenio>.Erro("Convênio não encontrado."));

            convenio.NomeConvenio = dto.NomeConvenio;
            convenio.TipoLeito = dto.TipoLeito;
            convenio.CobreInternacao = dto.CobreInternacao;
            convenio.CobreExames = dto.CobreExames;
            convenio.CobreCirurgia = dto.CobreCirurgia;
            convenio.LimiteMedicamento = dto.LimiteMedicamento;
            convenio.PercentualCobertura = dto.PercentualCobertura;
            convenio.Ativo = dto.Ativo;

            _repository.Update(convenio);
            await _repository.SaveChangesAsync();

            return Ok(ResultWrapper<string>.Ok(string.Empty, "Convênio atualizado com sucesso."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var convenio = await _repository.GetByIdAsync(id);
            if (convenio == null)
                return NotFound(ResultWrapper<Convenio>.Erro("Convênio não encontrado."));

            _repository.Delete(convenio);
            await _repository.SaveChangesAsync();

            return Ok(ResultWrapper<string>.Ok(string.Empty, "Convênio removido com sucesso."));
        }
    }
}