using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
namespace GestaoHospitalarApi.Application.Services
{
    public class ExameService : IExameService
    {
        private readonly IExameRepository _repository;
        public ExameService(IExameRepository repository) => _repository = repository;

        public async Task CadastrarAsync(ExameCadastroDto dto)
        {
            await _repository.AddAsync(new Exame { Nome = dto.Nome, Valor = dto.Valor, Descricao = dto.Descricao });
            await _repository.SaveChangesAsync();
        }

        public async Task AtualizarAsync(int id, ExameCadastroDto dto)
        {
            var exame = await _repository.GetByIdAsync(id);
            if (exame == null) throw new Exception("Exame não encontrado");
            exame.Nome = dto.Nome; exame.Valor = dto.Valor; exame.Descricao = dto.Descricao;
            _repository.Update(exame); await _repository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var exame = await _repository.GetByIdAsync(id);
            if (exame != null) { _repository.Delete(exame); await _repository.SaveChangesAsync(); }
        }

        public async Task<IEnumerable<ExameListagemDto>> ObterTodosAsync()
        {
            var exames = await _repository.GetAllAsync();
            return exames.Select(e => new ExameListagemDto { IdExame = e.IdExame, Nome = e.Nome, Valor = e.Valor ?? 0, Descricao = e.Descricao });
        }
    }
}