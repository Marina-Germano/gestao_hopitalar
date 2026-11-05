using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Application.Services
{
    public class SalaService : ISalaService
    {
        private readonly ISalaRepository _salaRepository;
        private readonly AppDbContext _context;

        public SalaService(ISalaRepository salaRepository, AppDbContext context)
        {
            _salaRepository = salaRepository;
            _context = context;
        }

        public async Task CadastrarAsync(SalaCadastroDto dto)
        {
            var sala = new Sala
            {
                Nome = dto.Nome,
                Tipo = dto.Tipo,
                IdAla = dto.IdAla,
                Status = "LIVRE" // Toda sala nova nasce livre
            };

            await _salaRepository.AddAsync(sala);
            await _salaRepository.SaveChangesAsync();
        }

        public async Task AtualizarAsync(int id, SalaCadastroDto dto)
        {
            var sala = await _salaRepository.GetByIdAsync(id);
            if (sala == null) throw new Exception("Sala não encontrada.");

            sala.Nome = dto.Nome;
            sala.Tipo = dto.Tipo;
            sala.IdAla = dto.IdAla;

            _salaRepository.Update(sala);
            await _salaRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<SalaListagemDto>> ObterTodasAsync()
        {
            var salas = await _salaRepository.ObterTodasComAlaAsync();
            return MapearLista(salas);
        }

        public async Task<IEnumerable<SalaListagemDto>> ObterLivresAsync()
        {
            var salas = await _salaRepository.ObterSalasLivresAsync();
            return MapearLista(salas);
        }

        public async Task<SalaCadastroDto?> ObterParaEdicaoAsync(int id)
        {
            var sala = await _salaRepository.ObterComAlaAsync(id);
            if (sala == null) return null;

            return new SalaCadastroDto
            {
                Nome = sala.Nome ?? "",
                Tipo = sala.Tipo ?? "",
                IdAla = sala.IdAla
            };
        }

        public async Task ExcluirAsync(int id)
        {
            var sala = await _salaRepository.GetByIdAsync(id);
            if (sala != null)
            {
                if (sala.Status != "LIVRE")
                    throw new Exception("Não é possível excluir uma sala que não esteja livre.");

                _salaRepository.Delete(sala);
                await _salaRepository.SaveChangesAsync();
            }
        }

        // Método auxiliar para não repetir código
        private IEnumerable<SalaListagemDto> MapearLista(IEnumerable<Sala> salas)
        {
            return salas.Select(s => new SalaListagemDto
            {
                IdSala = s.IdSala,
                Nome = s.Nome ?? "",
                Tipo = s.Tipo ?? "",
                Status = s.Status ?? "LIVRE",
                IdAla = s.IdAla,
                NomeAla = s.IdAlaNavigation?.NomeAla ?? "",
                //AndarAla = s.IdAlaNavigation?.Andar ?? ""
            });
        }

        public async Task<IEnumerable<object>> BuscarPacientesPorSalaAsync(int idSala)
        {
            // Busca prontuários ativos vinculados a esta sala
            var prontuarios = await _context.Set<Prontuario>()
                .Include(p => p.IdPacienteNavigation)
                    .ThenInclude(pac => pac.IdPessoaNavigation)
                .Where(p => p.IdSala == idSala && p.StatusProntuario == "ATIVO")
                .ToListAsync();

            // Mapeia para um formato genérico (Map<String, dynamic>) que o Flutter consiga ler
            return prontuarios.Select(p => new
            {
                id_prontuario = p.IdProntuario,
                id_paciente = p.IdPaciente,
                nome_paciente = p.IdPacienteNavigation?.IdPessoaNavigation?.Nome ?? "Paciente Desconhecido",
                // Você pode adicionar mais campos aqui se o ProntuarioEvolucaoForm precisar
            });
        }
    }
}