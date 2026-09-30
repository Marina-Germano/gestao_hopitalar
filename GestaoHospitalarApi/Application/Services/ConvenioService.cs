using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class ConvenioService : IConvenioService
    {
        private readonly IConvenioRepository _convenioRepository;

        public ConvenioService(IConvenioRepository convenioRepository)
        {
            _convenioRepository = convenioRepository;
        }

        public async Task CadastrarAsync(ConvenioCadastroDto dto)
        {
            var convenio = new Convenio
            {
                NomeConvenio = dto.NomeConvenio,
                // Mapeie outros campos caso existam no seu DTO de cadastro de convênio
            };

            await _convenioRepository.AddAsync(convenio);
            await _convenioRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ConvenioCadastroDto>> ObterTodosAsync()
        {
            var convenios = await _convenioRepository.GetAllAsync();

            return convenios.Select(c => new ConvenioCadastroDto
            {
                NomeConvenio = c.NomeConvenio ?? string.Empty
            });
        }

        public async Task ExcluirAsync(int id)
        {
            var convenio = await _convenioRepository.GetByIdAsync(id);
            if (convenio != null)
            {
                _convenioRepository.Delete(convenio);
                await _convenioRepository.SaveChangesAsync();
            }
        }
    }
}