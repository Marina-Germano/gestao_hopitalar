using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class SalaRepository : GenericRepository<Sala>, ISalaRepository
    {
        public SalaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Sala?> ObterComAlaAsync(int id)
        {
            return await _context.Set<Sala>()
                .Include(s => s.IdAlaNavigation)
                .FirstOrDefaultAsync(s => s.IdSala == id);
        }

        public async Task<IEnumerable<Sala>> ObterTodasComAlaAsync()
        {
            return await _context.Set<Sala>()
                .Include(s => s.IdAlaNavigation)
                .OrderBy(s => s.IdAlaNavigation.NomeAla)
                .ThenBy(s => s.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sala>> ObterSalasLivresAsync()
        {
            return await _context.Set<Sala>()
                .Include(s => s.IdAlaNavigation)
                .Where(s => s.Status == "LIVRE")
                .OrderBy(s => s.IdAlaNavigation.NomeAla)
                .ThenBy(s => s.Nome)
                .ToListAsync();
        }
    }
}