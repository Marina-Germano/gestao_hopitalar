using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class AlmoxarifadoService : IAlmoxarifadoService
    {
        private readonly IAlmoxarifadoRepository _almoxarifadoRepository;
        private readonly IMedicamentoRepository _medicamentoRepository;

        public AlmoxarifadoService(
            IAlmoxarifadoRepository almoxarifadoRepository,
            IMedicamentoRepository medicamentoRepository)
        {
            _almoxarifadoRepository = almoxarifadoRepository;
            _medicamentoRepository = medicamentoRepository;
        }

        public async Task CadastrarAsync(AlmoxarifadoCadastroDto dto)
        {
            var item = new Almoxarifado
            {
                Nome = dto.Nome,
                Categoria = dto.Categoria,
                Descricao = dto.Descricao,
                Quantidade = dto.Quantidade,
                Unidade = dto.Unidade,
                ValorUnitario = dto.ValorUnitario,
                EstoqueMinimo = dto.EstoqueMinimo,
                Lote = dto.Lote,
                Validade = dto.Validade.HasValue
                    ? DateOnly.FromDateTime(dto.Validade.Value)
                    : null
            };

            await _almoxarifadoRepository.AddAsync(item);
            await _almoxarifadoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AlmoxarifadoListagemDto>> ObterTodosAsync()
        {
            var itens = await _almoxarifadoRepository.ObterTodosComDetalhesAsync();
            return itens.Select(i => new AlmoxarifadoListagemDto
            {
                IdAlmoxarifado = i.IdAlmoxarifado,
                Nome = i.Nome,
                Categoria = i.Categoria,
                Descricao = i.Descricao,
                Quantidade = i.Quantidade,
                Unidade = i.Unidade,
                ValorUnitario = i.ValorUnitario,
                EstoqueMinimo = i.EstoqueMinimo,
                Lote = i.Lote,
                Validade = i.Validade?.ToDateTime(TimeOnly.MinValue)
            });
        }

        public async Task<AlmoxarifadoListagemDto?> ObterPorIdAsync(int id)
        {
            var i = await _almoxarifadoRepository.GetByIdAsync(id);
            if (i == null) return null;

            return new AlmoxarifadoListagemDto
            {
                IdAlmoxarifado = i.IdAlmoxarifado,
                Nome = i.Nome,
                Categoria = i.Categoria,
                Descricao = i.Descricao,
                Quantidade = i.Quantidade,
                Unidade = i.Unidade,
                ValorUnitario = i.ValorUnitario,
                EstoqueMinimo = i.EstoqueMinimo,
                Lote = i.Lote,
                Validade = i.Validade?.ToDateTime(TimeOnly.MinValue)
            };
        }

        public async Task AtualizarAsync(int id, AlmoxarifadoCadastroDto dto)
        {
            var item = await _almoxarifadoRepository.GetByIdAsync(id);
            if (item == null) throw new Exception("Item do almoxarifado não encontrado.");

            item.Nome = dto.Nome;
            item.Categoria = dto.Categoria;
            item.Descricao = dto.Descricao;
            item.Quantidade = dto.Quantidade;
            item.Unidade = dto.Unidade;
            item.ValorUnitario = dto.ValorUnitario;
            item.EstoqueMinimo = dto.EstoqueMinimo;
            item.Lote = dto.Lote;
            item.Validade = dto.Validade.HasValue
                ? DateOnly.FromDateTime(dto.Validade.Value)
                : null;

            _almoxarifadoRepository.Update(item);
            await _almoxarifadoRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            // 1. Remove primeiro a referência do medicamento filho (se for da categoria MEDICAMENTO)
            var medicamento = await _medicamentoRepository.ObterPorAlmoxarifadoIdAsync(id);
            if (medicamento != null)
            {
                _medicamentoRepository.Delete(medicamento);
                await _medicamentoRepository.SaveChangesAsync();
            }

            // 2. Exclui o item principal do almoxarifado
            var item = await _almoxarifadoRepository.GetByIdAsync(id);
            if (item == null) throw new Exception("Item do almoxarifado não encontrado.");

            _almoxarifadoRepository.Delete(item);
            await _almoxarifadoRepository.SaveChangesAsync();
        }
    }
}