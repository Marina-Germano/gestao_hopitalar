using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InternacoesController : ControllerBase
    {
        private readonly IGenericRepository<Internacao> _internacaoRepository;
        private readonly IGenericRepository<Prontuario> _prontuarioRepository;
        private readonly IGenericRepository<Leito> _leitoRepository;

        public InternacoesController(
            IGenericRepository<Internacao> internacaoRepository,
            IGenericRepository<Prontuario> prontuarioRepository,
            IGenericRepository<Leito> leitoRepository)
        {
            _internacaoRepository = internacaoRepository;
            _prontuarioRepository = prontuarioRepository;
            _leitoRepository = leitoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var internacoes = await _internacaoRepository.GetAllAsync();
            return Ok(internacoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var internacao = await _internacaoRepository.GetByIdAsync(id);
            if (internacao == null)
                return NotFound(new { mensagem = "Internação não encontrada." });

            return Ok(internacao);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InternacaoCadastroDto dto)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(dto.IdProntuario);
            if (prontuario == null)
                return NotFound(new { mensagem = "Prontuário informado não existe." });

            var leito = await _leitoRepository.GetByIdAsync(dto.IdLeito);
            if (leito == null)
                return NotFound(new { mensagem = "Leito informado não existe." });

            if (leito.Situacao != "VAGO")
                return BadRequest(new { mensagem = $"O leito {leito.Numero} não está vago. Situação atual: {leito.Situacao}" });

            var internacao = new Internacao
            {
                IdProntuario = dto.IdProntuario,
                IdLeito = dto.IdLeito,
                DataEntrada = DateTime.Now,
                Isolamento = dto.Isolamento.ToUpper(),
                StatusInternacao = "ATIVA"
            };

            // Atualiza a situação do leito para OCUPADO
            leito.Situacao = "OCUPADO";
            _leitoRepository.Update(leito);

            await _internacaoRepository.AddAsync(internacao);
            await _internacaoRepository.SaveChangesAsync();

            return StatusCode(201, new { mensagem = "Internação realizada com sucesso", idInternacao = internacao.IdInternacao });
        }

        [HttpPut("{id}/dar-alta")]
        public async Task<IActionResult> DarAlta(int id)
        {
            var internacao = await _internacaoRepository.GetByIdAsync(id);
            if (internacao == null)
                return NotFound(new { mensagem = "Internação não encontrada." });

            if (internacao.StatusInternacao == "ALTA")
                return BadRequest(new { mensagem = "Esta internação já teve alta registrada." });

            internacao.DataAlta = DateTime.Now;
            internacao.StatusInternacao = "ALTA";

            // Libera o leito e encaminha para higienização
            var leito = await _leitoRepository.GetByIdAsync(internacao.IdLeito);
            if (leito != null)
            {
                leito.Situacao = "HIGIENIZACAO";
                leito.DataHigienizacao = DateTime.Now;
                _leitoRepository.Update(leito);
            }

            _internacaoRepository.Update(internacao);
            await _internacaoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Alta médica registrada com sucesso. Leito liberado para higienização." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var internacao = await _internacaoRepository.GetByIdAsync(id);
            if (internacao == null)
                return NotFound(new { mensagem = "Internação não encontrada." });

            // Libera o leito caso a internação seja cancelada/excluída
            var leito = await _leitoRepository.GetByIdAsync(internacao.IdLeito);
            if (leito != null && leito.Situacao == "OCUPADO")
            {
                leito.Situacao = "VAGO";
                _leitoRepository.Update(leito);
            }

            _internacaoRepository.Delete(internacao);
            await _internacaoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Internação removida com sucesso." });
        }
    }
}