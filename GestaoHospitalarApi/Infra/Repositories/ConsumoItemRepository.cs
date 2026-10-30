using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;
namespace GestaoHospitalarApi.Infra.Repositories
{
    public class ConsumoItemRepository : GenericRepository<ConsumoItem>, IConsumoItemRepository
    {
        public ConsumoItemRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ConsumoItem>> ObterPorInternacaoAsync(int idInternacao)
        {
            return await _context.Set<ConsumoItem>()
                .Include(c => c.IdAlmoxarifadoNavigation)
                .Include(c => c.IdInternacaoNavigation)
                    .ThenInclude(i => i.IdProntuarioNavigation)
                        .ThenInclude(p => p.IdPacienteNavigation)
                .Where(c => c.IdInternacao == idInternacao)
                .OrderByDescending(c => c.DataConsumo)
                .ToListAsync();
        }
    }
}