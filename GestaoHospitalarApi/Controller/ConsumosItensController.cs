using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoHospitalarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumosItensController : ControllerBase
    {
        private readonly IGenericRepository<ConsumoItem> _consumoRepository;
        private readonly IGenericRepository<Internacao> _internacaoRepository;
        private readonly IGenericRepository<Almoxarifado> _almoxarifadoRepository;

        public ConsumosItensController(
            IGenericRepository<ConsumoItem> consumoRepository,
            IGenericRepository<Internacao> internacaoRepository,
            IGenericRepository<Almoxarifado> almoxarifadoRepository)
        {
            _consumoRepository = consumoRepository;
            _internacaoRepository = internacaoRepository;
            _almoxarifadoRepository = almoxarifadoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var consumos = await _consumoRepository.GetAllAsync();
            return Ok(consumos);
        }

        [HttpGet("internacao/{idInternacao}")]
        public async Task<IActionResult> GetByInternacao(int idInternacao)
        {
            var consumos = await _consumoRepository.FindAsync(c => c.IdInternacao == idInternacao);
            return Ok(consumos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConsumoItemCadastroDto dto)
        {
            if (dto.Quantidade <= 0)
                return BadRequest(new { mensagem = "A quantidade consumida deve ser maior que zero." });

            var internacao = await _internacaoRepository.GetByIdAsync(dto.IdInternacao);
            if (internacao == null)
                return NotFound(new { mensagem = "Internação informada não existe." });

            if (internacao.StatusInternacao != "ATIVA")
                return BadRequest(new { mensagem = "Não é possível lançar consumo em uma internação que já teve alta ou foi finalizada." });

            var itemEstoque = await _almoxarifadoRepository.GetByIdAsync(dto.IdAlmoxarifado);
            if (itemEstoque == null)
                return NotFound(new { mensagem = "Item de almoxarifado não encontrado." });

            // Validação de Estoque
            if (itemEstoque.Quantidade < dto.Quantidade)
            {
                return BadRequest(new { 
                    mensagem = $"Estoque insuficiente. Saldo atual do item '{itemEstoque.Nome}': {itemEstoque.Quantidade} {itemEstoque.Unidade}." 
                });
            }

            // 1. Dar baixa no Estoque do Almoxarifado
            itemEstoque.Quantidade -= dto.Quantidade;
            _almoxarifadoRepository.Update(itemEstoque);

            // 2. Registrar o consumo atrelado à Internação (Sem ValorUnitarioMomento)
            var consumo = new ConsumoItem
            {
                IdInternacao = dto.IdInternacao,
                IdAlmoxarifado = dto.IdAlmoxarifado,
                Quantidade = dto.Quantidade,
                DataConsumo = DateTime.Now,
            };

            await _consumoRepository.AddAsync(consumo);
            await _consumoRepository.SaveChangesAsync();

            return StatusCode(201, new { 
                mensagem = "Consumo registrado e estoque atualizado com sucesso", 
                idConsumo = consumo.IdConsumo, 
                saldoRestante = itemEstoque.Quantidade
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EstornarConsumo(int id)
        {
            var consumo = await _consumoRepository.GetByIdAsync(id);
            if (consumo == null)
                return NotFound(new { mensagem = "Registro de consumo não encontrado." });

            var internacao = await _internacaoRepository.GetByIdAsync(consumo.IdInternacao);
            if (internacao != null && internacao.StatusInternacao != "ATIVA")
            {
                return BadRequest(new { mensagem = "Não é possível estornar itens de uma internação já finalizada." });
            }

            // Estorna a quantidade de volta para o almoxarifado
            var itemEstoque = await _almoxarifadoRepository.GetByIdAsync(consumo.IdAlmoxarifado);
            if (itemEstoque != null)
            {
                itemEstoque.Quantidade += consumo.Quantidade;
                _almoxarifadoRepository.Update(itemEstoque);
            }

            _consumoRepository.Delete(consumo);
            await _consumoRepository.SaveChangesAsync();

            return Ok(new { mensagem = "Consumo estornado com sucesso e quantidade devolvida ao estoque." });
        }
    }
}