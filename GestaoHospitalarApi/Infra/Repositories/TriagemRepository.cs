using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class TriagemRepository : GenericRepository<Triagem>, ITriagemRepository
    {
        public TriagemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Triagem?> ObterComDetalhesAsync(int id)
        {
            return await _context.Set<Triagem>()
                .Include(t => t.IdPacienteNavigation)
                    .ThenInclude(p => p.IdPessoaNavigation)
                .FirstOrDefaultAsync(t => t.IdTriagem == id);
        }

        public async Task<IEnumerable<Triagem>> ObterTodasComDetalhesAsync()
        {
            // Triagens geralmente são ordenadas da mais recente para a mais antiga
            return await _context.Set<Triagem>()
                .Include(t => t.IdPacienteNavigation)
                    .ThenInclude(p => p.IdPessoaNavigation)
                //.OrderByDescending(t => t.DataTriagem)
                .ToListAsync();
        }
    }
}