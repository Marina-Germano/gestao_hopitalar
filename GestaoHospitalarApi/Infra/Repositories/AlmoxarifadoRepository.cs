using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Infra.EF;
using Microsoft.EntityFrameworkCore;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class AlmoxarifadoRepository : GenericRepository<Almoxarifado>, IAlmoxarifadoRepository
    {
        public AlmoxarifadoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Almoxarifado>> ObterTodosComDetalhesAsync()
        {
            return await _context.Set<Almoxarifado>()
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }
    }
}