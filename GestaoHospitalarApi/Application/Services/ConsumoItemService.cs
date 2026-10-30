using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
namespace GestaoHospitalarApi.Application.Services
{
    public class ConsumoItemService : IConsumoItemService
    {
        private readonly IConsumoItemRepository _consumoRepository;
        private readonly IGenericRepository<Almoxarifado> _almoxRepository;

        public ConsumoItemService(IConsumoItemRepository consumoRepository, IGenericRepository<Almoxarifado> almoxRepository)
        {
            _consumoRepository = consumoRepository;
            _almoxRepository = almoxRepository;
        }

        public async Task CadastrarAsync(ConsumoItemCadastroDto dto)
        {
            // Regra de Negócio: Dar baixa no estoque
            var itemEstoque = await _almoxRepository.GetByIdAsync(dto.IdAlmoxarifado);
            if (itemEstoque == null) 
                throw new Exception("Item não encontrado no almoxarifado.");
            
            if (itemEstoque.Quantidade < dto.Quantidade)
                throw new Exception($"Estoque insuficiente! Quantidade atual: {itemEstoque.Quantidade}");

            // Deduz a quantidade do estoque
            itemEstoque.Quantidade -= dto.Quantidade;
            _almoxRepository.Update(itemEstoque);

            // Registra o consumo
            var consumo = new ConsumoItem
            {
                IdInternacao = dto.IdInternacao,
                IdAlmoxarifado = dto.IdAlmoxarifado,
                Quantidade = dto.Quantidade,
                Observacao = dto.Observacao,
                DataConsumo = DateTime.Now
            };

            await _consumoRepository.AddAsync(consumo);
            await _consumoRepository.SaveChangesAsync(); // Salva a baixa no estoque e o registro do consumo na mesma transação
        }

        public async Task<IEnumerable<ConsumoItemListagemDto>> ObterPorInternacaoAsync(int idInternacao)
        {
            var consumos = await _consumoRepository.ObterPorInternacaoAsync(idInternacao);
            return consumos.Select(c => new ConsumoItemListagemDto
            {
                IdConsumo = c.IdConsumo,
                IdInternacao = c.IdInternacao,
                IdAlmoxarifado = c.IdAlmoxarifado,
                NomeItem = c.IdAlmoxarifadoNavigation?.Nome ?? "",
                NomePaciente = c.IdInternacaoNavigation?.IdProntuarioNavigation?.IdPacienteNavigation?.IdPessoaNavigation?.Nome ?? "",
                Quantidade = c.Quantidade,
                Observacao = c.Observacao,
                DataConsumo = c.DataConsumo
            });
        }

        public async Task<ConsumoItemListagemDto> ObterPorIdAsync(int id)
        {
            var c = await _consumoRepository.GetByIdAsync(id);
            if (c == null) throw new Exception("Consumo não encontrado.");
            return new ConsumoItemListagemDto
            {
                IdConsumo = c.IdConsumo, IdInternacao = c.IdInternacao, 
                IdAlmoxarifado = c.IdAlmoxarifado, Quantidade = c.Quantidade, 
                Observacao = c.Observacao, DataConsumo = c.DataConsumo
            };
        }

        public async Task AtualizarAsync(int id, ConsumoItemCadastroDto dto)
        {
            var consumo = await _consumoRepository.GetByIdAsync(id);
            if (consumo == null) throw new Exception("Consumo não encontrado.");
            
            var almox = await _almoxRepository.GetByIdAsync(consumo.IdAlmoxarifado);
            
            // Regra: Estorna o antigo e abate o novo
            almox.Quantidade += consumo.Quantidade; 
            if (almox.Quantidade < dto.Quantidade) throw new Exception("Estoque insuficiente para a nova quantidade.");
            almox.Quantidade -= dto.Quantidade;

            consumo.Quantidade = dto.Quantidade;
            consumo.Observacao = dto.Observacao;

            _almoxRepository.Update(almox);
            _consumoRepository.Update(consumo);
            await _consumoRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var consumo = await _consumoRepository.GetByIdAsync(id);
            if (consumo != null)
            {
                var almox = await _almoxRepository.GetByIdAsync(consumo.IdAlmoxarifado);
                if (almox != null)
                {
                    // Devolve o item para o estoque ao excluir
                    almox.Quantidade += consumo.Quantidade;
                    _almoxRepository.Update(almox);
                }
                _consumoRepository.Delete(consumo);
                await _consumoRepository.SaveChangesAsync();
            }
        }
    }
}