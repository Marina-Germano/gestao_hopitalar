using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;
namespace GestaoHospitalarApi.Infra.Repositories
{
    public class InteracaoRepository : GenericRepository<InteracaoMedicamentosa>, IInteracaoRepository
    {
        public InteracaoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<InteracaoMedicamentosa>> ObterTodasComDetalhesAsync()
        {
            return await _context.Set<InteracaoMedicamentosa>()
                .Include(i => i.IdMedicamento1Navigation)
                    .ThenInclude(m => m.IdAlmoxarifadoNavigation)
                .Include(i => i.IdMedicamento2Navigation)
                    .ThenInclude(m => m.IdAlmoxarifadoNavigation)
                .ToListAsync();
        }
    }
}