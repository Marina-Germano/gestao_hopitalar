using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Wrappers;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitacoesExamesController : ControllerBase
    {
        private readonly IGenericRepository<SolicitacaoExame> _solicitacaoRepository;
        private readonly IGenericRepository<Prontuario> _prontuarioRepository;
        private readonly IGenericRepository<Exame> _exameRepository;
        private readonly IGenericRepository<Medico> _medicoRepository;

        public SolicitacoesExamesController(
            IGenericRepository<SolicitacaoExame> solicitacaoRepository,
            IGenericRepository<Prontuario> prontuarioRepository,
            IGenericRepository<Exame> exameRepository,
            IGenericRepository<Medico> medicoRepository)
        {
            _solicitacaoRepository = solicitacaoRepository;
            _prontuarioRepository = prontuarioRepository;
            _exameRepository = exameRepository;
            _medicoRepository = medicoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var solicitacoes = await _solicitacaoRepository.GetAllAsync();
            return Ok(ResultWrapper<IEnumerable<SolicitacaoExame>>.Ok(solicitacoes, "Solicitações de exame recuperadas com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var solicitacao = await _solicitacaoRepository.GetByIdAsync(id);
            if (solicitacao == null)
                return NotFound(ResultWrapper<SolicitacaoExame>.Erro("Solicitação de exame não encontrada."));

            return Ok(ResultWrapper<SolicitacaoExame>.Ok(solicitacao, "Solicitação de exame recuperada com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SolicitacaoExameCadastroDto dto)
        {
            var prontuario = await _prontuarioRepository.GetByIdAsync(dto.IdProntuario);
            if (prontuario == null)
                return NotFound(ResultWrapper<Prontuario>.Erro("Prontuário informado não existe."));

            var exame = await _exameRepository.GetByIdAsync(dto.IdExame);
            if (exame == null)
                return NotFound(ResultWrapper<Exame>.Erro("Exame informado não existe."));

            var medico = await _medicoRepository.GetByIdAsync(dto.IdMedico);
            if (medico == null)
                return NotFound(ResultWrapper<Medico>.Erro("Médico informado não existe."));

            var solicitacao = new SolicitacaoExame
            {
                IdProntuario = dto.IdProntuario,
                IdExame = dto.IdExame,
                IdMedico = dto.IdMedico,
                DataSolicitacao = DateTime.Now,
                StatusExame = string.IsNullOrEmpty(dto.Status) ? "SOLICITADO" : dto.Status.ToUpper(),
                Resultado = dto.Resultado
            };

            await _solicitacaoRepository.AddAsync(solicitacao);
            await _solicitacaoRepository.SaveChangesAsync();

            return StatusCode(201, ResultWrapper<object>.Ok(new { mensagem = "Solicitação de exame criada com sucesso", idSolicitacao = solicitacao.IdSolicitacao }, "Solicitação de exame criada com sucesso"));
        }

        [HttpPut("{id}/resultado")]
        public async Task<IActionResult> RegistrarResultado(int id, [FromBody] string resultado)
        {
            var solicitacao = await _solicitacaoRepository.GetByIdAsync(id);
            if (solicitacao == null)
                return NotFound(ResultWrapper<SolicitacaoExame>.Erro("Solicitação de exame não encontrada."));

            solicitacao.Resultado = resultado;
            solicitacao.StatusExame = "REALIZADO";

            _solicitacaoRepository.Update(solicitacao);
            await _solicitacaoRepository.SaveChangesAsync();

            return Ok(ResultWrapper<object>.Ok(new { mensagem = "Resultado do exame registrado com sucesso." }, "Resultado do exame registrado com sucesso."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var solicitacao = await _solicitacaoRepository.GetByIdAsync(id);
            if (solicitacao == null)
                return NotFound(ResultWrapper<SolicitacaoExame>.Erro("Solicitação de exame não encontrada."));

            _solicitacaoRepository.Delete(solicitacao);
            await _solicitacaoRepository.SaveChangesAsync();

            return Ok(ResultWrapper<string>.Ok(string.Empty, "Solicitação de exame removida com sucesso."));
        }
    }
}