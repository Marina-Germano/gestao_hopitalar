using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class AlaRepository : GenericRepository<Ala>, IAlaRepository
    {
        public AlaRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Ala>> ObterTodasOrdenadasAsync()
        {
            return await _context.Set<Ala>()
                .OrderBy(a => a.NomeAla)
                .ToListAsync();
        }
    }
}