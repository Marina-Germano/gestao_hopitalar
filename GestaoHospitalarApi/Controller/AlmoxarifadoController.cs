using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Wrappers;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlmoxarifadoController : ControllerBase
    {
        private readonly IGenericRepository<Almoxarifado> _repository;

        public AlmoxarifadoController(IGenericRepository<Almoxarifado> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var itens = await _repository.GetAllAsync();
            return Ok(ResultWrapper<IEnumerable<Almoxarifado>>.Ok(itens, "Itens do almoxarifado recuperados com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return NotFound(ResultWrapper<Almoxarifado>.Erro("Item do almoxarifado não encontrado."));

            return Ok(ResultWrapper<Almoxarifado>.Ok(item, "Item do almoxarifado recuperado com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AlmoxarifadoCadastroDto dto)
        {
            var item = new Almoxarifado
            {
                Nome = dto.Nome,
                Categoria = dto.Categoria.ToUpper(),
                Descricao = dto.Descricao,
                Quantidade = dto.Quantidade,
                Unidade = dto.Unidade,
                ValorUnitario = dto.ValorUnitario,
                EstoqueMinimo = dto.EstoqueMinimo ?? 0,
                Lote = dto.Lote,
                Validade = dto.Validade.HasValue
                    ? DateOnly.FromDateTime(dto.Validade.Value)
                    : null
            };

            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            return StatusCode(201, ResultWrapper<Almoxarifado>.Ok(item, "Item cadastrado no almoxarifado com sucesso"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AlmoxarifadoCadastroDto dto)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return NotFound(ResultWrapper<Almoxarifado>.Erro("Item não encontrado."));

            item.Nome = dto.Nome;
            item.Categoria = dto.Categoria.ToUpper();
            item.Descricao = dto.Descricao;
            item.Quantidade = dto.Quantidade;
            item.Unidade = dto.Unidade;
            item.ValorUnitario = dto.ValorUnitario;
            item.EstoqueMinimo = dto.EstoqueMinimo ?? 0;
            item.Lote = dto.Lote;
            item.Validade = dto.Validade.HasValue
                ? DateOnly.FromDateTime(dto.Validade.Value)
                : null;

            _repository.Update(item);
            await _repository.SaveChangesAsync();

            return Ok(ResultWrapper<Almoxarifado>.Ok(item, "Item do almoxarifado atualizado com sucesso."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return NotFound(ResultWrapper<Almoxarifado>.Erro("Item não encontrado."));

            _repository.Delete(item);
            await _repository.SaveChangesAsync();

            return Ok(ResultWrapper<string>.Ok(string.Empty, "Item do almoxarifado removido com sucesso."));
        }
    }
}