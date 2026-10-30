using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class FaturamentoRepository : GenericRepository<Faturamento>, IFaturamentoRepository
    {
        public FaturamentoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Faturamento>> ObterTodosComDetalhesAsync()
        {
            return await _context.Set<Faturamento>()
                .Include(f => f.IdInternacaoNavigation)
                    .ThenInclude(i => i.IdProntuarioNavigation)
                        .ThenInclude(p => p.IdPacienteNavigation)
                .OrderByDescending(f => f.DataFechamento)
                .ToListAsync();
        }

        public async Task<Faturamento?> ObterPorInternacaoAsync(int idInternacao)
        {
            // O parâmetro idInternacao agora está mapeado perfeitamente
            return await _context.Set<Faturamento>()
                .FirstOrDefaultAsync(f => f.IdInternacao == idInternacao);
        }
    }
}