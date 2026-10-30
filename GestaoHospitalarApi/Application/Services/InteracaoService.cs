using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
namespace GestaoHospitalarApi.Application.Services
{
    public class InteracaoService : IInteracaoService
    {
        private readonly IInteracaoRepository _repository;
        public InteracaoService(IInteracaoRepository repository) => _repository = repository;

        public async Task CadastrarAsync(InteracaoCadastroDto dto)
        {
            var interacao = new InteracaoMedicamentosa
            {
                IdMedicamento1 = dto.IdMedicamento1,
                IdMedicamento2 = dto.IdMedicamento2,
                Gravidade = dto.Gravidade,
                Descricao = dto.Descricao
            };
            await _repository.AddAsync(interacao);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<InteracaoListagemDto>> ObterTodasAsync()
        {
            var interacoes = await _repository.ObterTodasComDetalhesAsync();
            return interacoes.Select(i => new InteracaoListagemDto
            {
                IdInteracao = i.IdInteracao,
                IdMedicamento1 = i.IdMedicamento1,
                NomeMedicamento1 = i.IdMedicamento1Navigation?.IdAlmoxarifadoNavigation?.Nome ?? "",
                IdMedicamento2 = i.IdMedicamento2,
                NomeMedicamento2 = i.IdMedicamento2Navigation?.IdAlmoxarifadoNavigation?.Nome ?? "",
                Gravidade = i.Gravidade ?? "",
                Descricao = i.Descricao
            });
        }

        public async Task<InteracaoListagemDto> ObterPorIdAsync(int id)
        {
            var i = await _repository.GetByIdAsync(id);
            if (i == null) throw new Exception("Interação não encontrada.");
            return new InteracaoListagemDto
            {
                IdInteracao = i.IdInteracao, IdMedicamento1 = i.IdMedicamento1, 
                IdMedicamento2 = i.IdMedicamento2, Gravidade = i.Gravidade ?? "", 
                Descricao = i.Descricao
            };
        }

        public async Task AtualizarAsync(int id, InteracaoCadastroDto dto)
        {
            var interacao = await _repository.GetByIdAsync(id);
            if (interacao == null) throw new Exception("Interação não encontrada.");

            interacao.IdMedicamento1 = dto.IdMedicamento1;
            interacao.IdMedicamento2 = dto.IdMedicamento2;
            interacao.Gravidade = dto.Gravidade;
            interacao.Descricao = dto.Descricao;

            _repository.Update(interacao);
            await _repository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var interacao = await _repository.GetByIdAsync(id);
            if (interacao != null)
            {
                _repository.Delete(interacao);
                await _repository.SaveChangesAsync();
            }
        }
    }
}