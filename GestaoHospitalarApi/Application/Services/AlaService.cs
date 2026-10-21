using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class AlaService : IAlaService
    {
        private readonly IAlaRepository _alaRepository;

        public AlaService(IAlaRepository alaRepository)
        {
            _alaRepository = alaRepository;
        }

        public async Task CadastrarAsync(AlaCadastroDto dto)
        {
            var ala = new Ala
            {
                NomeAla = dto.NomeAla, // Ajuste para a propriedade do seu DTO
                Andar = dto.Andar
            };
            await _alaRepository.AddAsync(ala);
            await _alaRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AlaListagemDto>> ObterTodasAsync()
        {
            var alas = await _alaRepository.ObterTodasOrdenadasAsync();
            return alas.Select(a => new AlaListagemDto
            {
                IdAla = a.IdAla,
                NomeAla = a.NomeAla ?? "",
                Andar = a.Andar ?? ""
            });
        }

        public async Task AtualizarAsync(int id, AlaCadastroDto dto)
        {
            var ala = await _alaRepository.GetByIdAsync(id);
            if (ala == null) throw new Exception("Ala não encontrada.");

            ala.NomeAla = dto.NomeAla;
            ala.Andar = dto.Andar;

            _alaRepository.Update(ala);
            await _alaRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var ala = await _alaRepository.GetByIdAsync(id);
            if (ala != null)
            {
                _alaRepository.Delete(ala);
                await _alaRepository.SaveChangesAsync();
            }
        }
    }
}