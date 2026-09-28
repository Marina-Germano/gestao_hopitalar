using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescricoesController : ControllerBase
    {
        private readonly IGenericRepository<Prescricao> _prescricaoRepository;
        private readonly IGenericRepository<Prontuario> _prontuarioRepository;
        private readonly IGenericRepository<Medico> _medicoRepository;
        private readonly IGenericRepository<Medicamento> _medicamentoRepository;

        public PrescricoesController(
            IGenericRepository<Prescricao> prescricaoRepository,
            IGenericRepository<Prontuario> prontuarioRepository,
            IGenericRepository<Medico> medicoRepository,
            IGenericRepository<Medicamento> medicamentoRepository)
        {
            _prescricaoRepository = prescricaoRepository;
            _prontuarioRepository = prontuarioRepository;
            _medicoRepository = medicoRepository;
            _medicamentoRepository = medicamentoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var prescricoes = await _prescricaoRepository.GetAllAsync();
            return Ok(prescricoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prescricao = await _prescricaoRepository.GetByIdAsync(id);
            if (prescricao == null)
                return NotFound(new { mensagem = "Prescrição não encontrada." });

            return Ok(prescricao);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrescricaoCadastroDto dto)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(dto.IdProntuario);
            if (prontuario == null)
                return NotFound(new { mensagem = "Prontuário informado não existe." });

            var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);
            if (medico == null)
                return NotFound(new { mensagem = "Médico informado não existe." });

            var medicamento = await _medicamentoRepository.GetByIdAsync(dto.IdMedicamento);
            if (medicamento == null)
                return NotFound(new { mensagem = "Medicamento informado não existe." });

            var prescricao = new Prescricao
            {
                IdProntuario = dto.IdProntuario,
                IdMedico = dto.IdMedico,
                IdMedicamento = dto.IdMedicamento,
                Dosagem = dto.Dosagem,
                Observacao = dto.Observacao,
                //DataPrescricao = DateTime.Now,
            };

            await _prescricaoRepository.AddAsync(prescricao);
            await _prescricaoRepository.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Prescrição criada com sucesso", idPrescricao = prescricao.IdPrescricao });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PrescricaoCadastroDto dto)
        {
            var prescricao = await _prescricaoRepository.GetByIdAsync(id);
            if (prescricao == null)
                return NotFound(new { mensagem = "Prescrição não encontrada." });

            prescricao.Dosagem = dto.Dosagem;
            prescricao.Observacao = dto.Observacao;
            // prescricao.Status = dto.Status.ToUpper();

            _prescricaoRepository.Update(prescricao);
            await _prescricaoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Prescrição atualizada com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var prescricao = await _prescricaoRepository.GetByIdAsync(id);
            if (prescricao == null)
                return NotFound(new { mensagem = "Prescrição não encontrada." });

            _prescricaoRepository.Delete(prescricao);
            await _prescricaoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Prescrição removida com sucesso." });
        }
    }
}