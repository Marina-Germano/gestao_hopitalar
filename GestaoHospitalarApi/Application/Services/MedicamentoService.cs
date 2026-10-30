using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
namespace GestaoHospitalarApi.Application.Services
{
    public class MedicamentoService : IMedicamentoService
    {
        private readonly IMedicamentoRepository _medRepository;
        private readonly IGenericRepository<Almoxarifado> _almoxRepository;

        public MedicamentoService(IMedicamentoRepository medRepository, IGenericRepository<Almoxarifado> almoxRepository)
        {
            _medRepository = medRepository;
            _almoxRepository = almoxRepository;
        }

        public async Task CadastrarAsync(MedicamentoCadastroDto dto)
        {
            // 1. Cadastra no Estoque primeiro
            var almoxarifado = new Almoxarifado
            {
                Nome = dto.Nome, Categoria = "MEDICAMENTO", Descricao = dto.Descricao,
                Quantidade = dto.Quantidade, Unidade = dto.Unidade, ValorUnitario = dto.ValorUnitario,
                EstoqueMinimo = dto.EstoqueMinimo, Lote = dto.Lote,
                Validade = dto.Validade.HasValue ? DateOnly.FromDateTime(dto.Validade.Value) : null
            };
            await _almoxRepository.AddAsync(almoxarifado);
            await _almoxRepository.SaveChangesAsync(); // Gera o ID

            // 2. Cadastra os dados clínicos atrelados ao estoque
            var medicamento = new Medicamento
            {
                IdAlmoxarifado = almoxarifado.IdAlmoxarifado,
                PrincipioAtivo = dto.PrincipioAtivo,
                Contraindicacoes = dto.Contraindicacoes
            };
            await _medRepository.AddAsync(medicamento);
            await _medRepository.SaveChangesAsync();
        }

        public async Task AtualizarAsync(int id, MedicamentoCadastroDto dto)
        {
            var med = await _medRepository.GetByIdAsync(id);
            if (med == null) throw new Exception("Medicamento não encontrado");
            var almox = await _almoxRepository.GetByIdAsync(med.IdAlmoxarifado);

            // Atualiza Estoque
            almox.Nome = dto.Nome; almox.Descricao = dto.Descricao; almox.Quantidade = dto.Quantidade;
            almox.ValorUnitario = dto.ValorUnitario; almox.EstoqueMinimo = dto.EstoqueMinimo;
            _almoxRepository.Update(almox);

            // Atualiza Clínica
            med.PrincipioAtivo = dto.PrincipioAtivo; med.Contraindicacoes = dto.Contraindicacoes;
            _medRepository.Update(med);

            await _medRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var med = await _medRepository.GetByIdAsync(id);
            if (med != null)
            {
                var almox = await _almoxRepository.GetByIdAsync(med.IdAlmoxarifado);
                _medRepository.Delete(med);
                if (almox != null) _almoxRepository.Delete(almox);
                await _medRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MedicamentoListagemDto>> ObterTodosAsync()
        {
            var meds = await _medRepository.ObterTodosComAlmoxarifadoAsync();
            return meds.Select(m => new MedicamentoListagemDto
            {
                IdMedicamento = m.IdMedicamento,
                IdAlmoxarifado = m.IdAlmoxarifado,
                Nome = m.IdAlmoxarifadoNavigation.Nome,
                Categoria = m.IdAlmoxarifadoNavigation.Categoria,
                Quantidade = m.IdAlmoxarifadoNavigation.Quantidade,
                EstoqueMinimo = m.IdAlmoxarifadoNavigation.EstoqueMinimo ?? 0,
                PrincipioAtivo = m.PrincipioAtivo,
                Contraindicacoes = m.Contraindicacoes
            });
        }
    }
}