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
            return await _context.Set<Triagem>()
                .Include(t => t.IdPacienteNavigation)
                    .ThenInclude(p => p.IdPessoaNavigation)
                .Where(t => t.Internacao == "NAO" || t.Internacao == null) // 🟢 Filtra apenas quem ainda não foi encaminhado
                .ToListAsync();
        }
    }
}