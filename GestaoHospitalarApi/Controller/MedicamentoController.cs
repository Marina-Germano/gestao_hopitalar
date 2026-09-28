using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
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
            return Ok(medicamentos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medicamento = await _medicamentoRepository.GetByIdAsync(id);
            if (medicamento == null)
                return NotFound(new { mensagem = "Medicamento não encontrado." });

            return Ok(medicamento);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicamentoCadastroDto dto)
        {
            var itemAlmoxarifado = await _almoxarifadoRepository.GetByIdAsync(dto.IdAlmoxarifado);
            if (itemAlmoxarifado == null)
                return NotFound(new { mensagem = "Item do almoxarifado informado não existe." });

            var medicamento = new Medicamento
            {
                IdAlmoxarifado = dto.IdAlmoxarifado,
                PrincipioAtivo = dto.PrincipioAtivo,
                Contraindicacoes = dto.Contraindicacoes
            };

            await _medicamentoRepository.AddAsync(medicamento);
            await _medicamentoRepository.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Medicamento registrado com sucesso", idMedicamento = medicamento.IdMedicamento });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicamentoCadastroDto dto)
        {
            var medicamento = await _medicamentoRepository.GetByIdAsync(id);
            if (medicamento == null)
                return NotFound(new { mensagem = "Medicamento não encontrado." });

            medicamento.IdAlmoxarifado = dto.IdAlmoxarifado;
            medicamento.PrincipioAtivo = dto.PrincipioAtivo;
            medicamento.Contraindicacoes = dto.Contraindicacoes;

            _medicamentoRepository.Update(medicamento);
            await _medicamentoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Medicamento atualizado com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var medicamento = await _medicamentoRepository.GetByIdAsync(id);
            if (medicamento == null)
                return NotFound(new { mensagem = "Medicamento não encontrado." });

            _medicamentoRepository.Delete(medicamento);
            await _medicamentoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Medicamento removido com sucesso." });
        }
    }
}