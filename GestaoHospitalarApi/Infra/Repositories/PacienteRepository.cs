using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
// Ajuste o namespace do seu DbContext se necessário:
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class PacienteRepository : GenericRepository<Paciente>, IPacienteRepository
    {
        public PacienteRepository(AppDbContext context) : base(context)
        {
        }

        // Traz o paciente com Pessoa, PacienteConvenio e o Convênio em uma única query
        public async Task<Paciente?> ObterComDetalhesAsync(int id)
        {
            return await _context.Set<Paciente>()
                .Include(p => p.IdPessoaNavigation)
                .Include(p => p.PacienteConvenios)
                    .ThenInclude(pc => pc.IdConvenioNavigation)
                .FirstOrDefaultAsync(p => p.IdPaciente == id);
        }

        // Traz a lista de pacientes (Ativos ou Inativos) juntamente com a Pessoa para pegar Nome e CPF
        public async Task<IEnumerable<Paciente>> ObterTodosComPessoaAsync(int ativo)
        {
            return await _context.Set<Paciente>()
                .Include(p => p.IdPessoaNavigation)
                .Where(p => p.Ativo == ativo)
                .ToListAsync();
        }
    }
}