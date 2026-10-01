using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class EspecialidadeService : IEspecialidadeService
    {
        private readonly IEspecialidadeRepository _especialidadeRepository;

        public EspecialidadeService(IEspecialidadeRepository especialidadeRepository)
        {
            _especialidadeRepository = especialidadeRepository;
        }

        public async Task CadastrarAsync(EspecialidadeCadastroDto dto)
        {
            var especialidade = new Especialidade
            {
                DescricaoEspecialidade = dto.DescricaoEspecialidade,
                // Mapeie outros campos caso existam no seu DTO de cadastro de especialidade
            };

            await _especialidadeRepository.AddAsync(especialidade);
            await _especialidadeRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<EspecialidadeCadastroDto>> ObterTodosAsync()
        {
            var especialidades = await _especialidadeRepository.GetAllAsync();

            return especialidades.Select(e => new EspecialidadeCadastroDto
            {
                DescricaoEspecialidade = e.DescricaoEspecialidade
            });
        }

        public async Task ExcluirAsync(int id)
        {
            var especialidade = await _especialidadeRepository.GetByIdAsync(id);
            if (especialidade != null)
            {
                _especialidadeRepository.Delete(especialidade);
                await _especialidadeRepository.SaveChangesAsync();
            }
        }
    }
}