using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class LeitoRepository : GenericRepository<Leito>, ILeitoRepository
    {
        public LeitoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Leito?> ObterComAlaAsync(int id)
        {
            return await _context.Set<Leito>()
                .Include(l => l.IdAlaNavigation)
                .FirstOrDefaultAsync(l => l.IdLeito == id);
        }

        public async Task<IEnumerable<Leito>> ObterTodosComAlaAsync()
        {
            return await _context.Set<Leito>()
                .Include(l => l.IdAlaNavigation)
                .OrderBy(l => l.IdAlaNavigation.NomeAla) // Usa o nome exato gerado pelo banco
                .ThenBy(l => l.Numero)
                .ToListAsync();
        }

        public async Task<IEnumerable<Leito>> ObterLeitosLivresAsync()
        {
            return await _context.Set<Leito>()
                .Include(l => l.IdAlaNavigation)
                .Where(l => l.Situacao == "Vago" || l.Situacao == "LIVRE" || l.Situacao == "VAGO")
                .OrderBy(l => l.IdAlaNavigation.NomeAla)
                .ThenBy(l => l.Numero)
                .ToListAsync();
        }

        public async Task<IEnumerable<Leito>> ObterLeitosVagosAsync()
        {
            return await ObterLeitosLivresAsync();
        }
    }
}