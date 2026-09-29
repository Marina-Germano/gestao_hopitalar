using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Application.Wrappers;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaturamentosController : ControllerBase
    {
        private readonly IGenericRepository<Internacao> _internacaoRepository;
        private readonly IGenericRepository<ConsumoItem> _consumoRepository;
        private readonly IGenericRepository<Almoxarifado> _almoxarifadoRepository;
        private readonly IGenericRepository<SolicitacaoExame> _solicitacaoRepository;
        private readonly IGenericRepository<Exame> _exameRepository;

        private const decimal VALOR_DIARIA_PADRAO = 250.00m; 

        public FaturamentosController(
            IGenericRepository<Internacao> internacaoRepository,
            IGenericRepository<ConsumoItem> consumoRepository,
            IGenericRepository<Almoxarifado> almoxarifadoRepository,
            IGenericRepository<SolicitacaoExame> solicitacaoRepository,
            IGenericRepository<Exame> exameRepository)
        {
            _internacaoRepository = internacaoRepository;
            _consumoRepository = consumoRepository;
            _almoxarifadoRepository = almoxarifadoRepository;
            _solicitacaoRepository = solicitacaoRepository;
            _exameRepository = exameRepository;
        }

        [HttpGet("calcular/internacao/{idInternacao}")]
        public async Task<IActionResult> CalcularFaturamentoFinal(int idInternacao)
        {
            var internacao = await _internacaoRepository.GetByIdAsync(idInternacao);
            if (internacao == null)
                return NotFound(ResultWrapper<Internacao>.Erro("Internação não encontrada."));

            var relatorio = new FaturamentoRelatorioDto
            {
                IdInternacao = internacao.IdInternacao,
                IdProntuario = internacao.IdProntuario,
                DataEntrada = internacao.DataEntrada,
                DataAlta = internacao.DataAlta
            };

            DateTime dataCalculoFim = internacao.DataAlta ?? DateTime.Now;
            int diasInternado = (dataCalculoFim.Date - internacao.DataEntrada.Date).Days;
            if (diasInternado == 0) diasInternado = 1; 

            relatorio.TotalDiarias = diasInternado;
            relatorio.ValorTotalDiarias = diasInternado * VALOR_DIARIA_PADRAO;

            var consumos = await _consumoRepository.FindAsync(c => c.IdInternacao == idInternacao);
            decimal totalConsumo = 0;

            foreach (var consumo in consumos)
            {
                // Busca o item no almoxarifado para pegar o valor atualizado e o nome
                var itemEstoque = await _almoxarifadoRepository.GetByIdAsync(consumo.IdAlmoxarifado);
                var nomeItem = itemEstoque?.Nome ?? "Item Desconhecido";
                decimal valorUnitarioAtual = itemEstoque?.ValorUnitario ?? 0m; // Usando o valor atual do almoxarifado

                var valorTotalItem = consumo.Quantidade * valorUnitarioAtual;
                totalConsumo += valorTotalItem;

                relatorio.DetalhesConsumo.Add(new ItemConsumoFaturamentoDto
                {
                    NomeItem = nomeItem,
                    Quantidade = consumo.Quantidade,
                    ValorUnitario = valorUnitarioAtual,
                    ValorTotalItem = valorTotalItem
                });
            }
            relatorio.ValorTotalConsumo = totalConsumo;

            var solicitacoes = await _solicitacaoRepository.FindAsync(s => s.IdProntuario == internacao.IdProntuario);
            decimal totalExames = 0;

            foreach (var solicitacao in solicitacoes)
            {
                var exame = await _exameRepository.GetByIdAsync(solicitacao.IdExame);
                if (exame != null)
                {
                    decimal valorExame = exame.Valor ?? 0;
                    totalExames += valorExame;

                    relatorio.DetalhesExames.Add(new ItemExameFaturamentoDto
                    {
                        NomeExame = exame.Nome,
                        ValorExame = valorExame
                    });
                }
            }
            relatorio.ValorTotalExames = totalExames;

            relatorio.ValorTotalGeral = relatorio.ValorTotalDiarias + relatorio.ValorTotalConsumo + relatorio.ValorTotalExames;

            return Ok(ResultWrapper<FaturamentoRelatorioDto>.Ok(relatorio, "Faturamento calculado com sucesso."));
        }
    }
}