using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Wrappers;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentosController : ControllerBase
    {
        private readonly IGenericRepository<Medicamento> _medicamentoRepository;
        private readonly IGenericRepository<Almoxarifado> _almoxarifadoRepository;

        public MedicamentosController(
            IGenericRepository<Medicamento> medicamentoRepository,
            IGenericRepository<Almoxarifado> almoxarifadoRepository)
        {
            _medicamentoRepository = medicamentoRepository;
            _almoxarifadoRepository = almoxarifadoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var medicamentos = await _medicamentoRepository.GetAllAsync();
            return Ok(ResultWrapper<IEnumerable<Medicamento>>.Ok(medicamentos, "Medicamentos recuperados com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medicamento = await _medicamentoRepository.GetByIdAsync(id);
            if (medicamento == null)
                return NotFound(ResultWrapper<Medicamento>.Erro("Medicamento não encontrado."));

            return Ok(ResultWrapper<Medicamento>.Ok(medicamento, "Medicamento recuperado com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicamentoCadastroDto dto)
        {
            var itemAlmoxarifado = await _almoxarifadoRepository.GetByIdAsync(dto.IdAlmoxarifado);
            if (itemAlmoxarifado == null)
                return NotFound(ResultWrapper<Almoxarifado>.Erro("Item do almoxarifado informado não existe."));

            var medicamento = new Medicamento
            {
                IdAlmoxarifado = dto.IdAlmoxarifado,
                PrincipioAtivo = dto.PrincipioAtivo,
                Contraindicacoes = dto.Contraindicacoes
            };

            await _medicamentoRepository.AddAsync(medicamento);
            await _medicamentoRepository.SaveChangesAsync();

            return StatusCode(201, ResultWrapper<object>.Ok(new { mensagem = "Medicamento registrado com sucesso", idMedicamento = medicamento.IdMedicamento }, "Medicamento registrado com sucesso"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicamentoCadastroDto dto)
        {
            var medicamento = await _medicamentoRepository.GetByIdAsync(id);
            if (medicamento == null)
                return NotFound(ResultWrapper<Medicamento>.Erro("Medicamento não encontrado."));

            medicamento.IdAlmoxarifado = dto.IdAlmoxarifado;
            medicamento.PrincipioAtivo = dto.PrincipioAtivo;
            medicamento.Contraindicacoes = dto.Contraindicacoes;

            _medicamentoRepository.Update(medicamento);
            await _medicamentoRepository.SaveChangesAsync();

            return Ok(ResultWrapper<object>.Ok(new { mensagem = "Medicamento atualizado com sucesso." }, "Medicamento atualizado com sucesso."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var medicamento = await _medicamentoRepository.GetByIdAsync(id);
            if (medicamento == null)
                return NotFound(ResultWrapper<Medicamento>.Erro("Medicamento não encontrado."));

            _medicamentoRepository.Delete(medicamento);
            await _medicamentoRepository.SaveChangesAsync();

            return Ok(ResultWrapper<string>.Ok(string.Empty, "Medicamento removido com sucesso."));
        }
    }
}