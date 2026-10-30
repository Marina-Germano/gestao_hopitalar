using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Infra.EF;
using Microsoft.EntityFrameworkCore;
namespace GestaoHospitalarApi.Infra.Repositories
{
    public class MedicamentoRepository : GenericRepository<Medicamento>, IMedicamentoRepository
    {
        public MedicamentoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Medicamento>> ObterTodosComAlmoxarifadoAsync()
        {
            return await _context.Set<Medicamento>()
                .Include(m => m.IdAlmoxarifadoNavigation)
                .OrderBy(m => m.IdAlmoxarifadoNavigation.Nome)
                .ToListAsync();
        }
    }
}