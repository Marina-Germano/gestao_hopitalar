using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class SolicitacaoExameRepository : GenericRepository<SolicitacaoExame>, ISolicitacaoExameRepository
    {
        public SolicitacaoExameRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<SolicitacaoExame>> ObterPorProntuarioAsync(int idProntuario)
        {
            return await _context.Set<SolicitacaoExame>()
                .Include(s => s.IdMedicoNavigation)
                    .ThenInclude(m => m.IdUsuarioNavigation)
                        .ThenInclude(u => u.IdPessoaNavigation)
                .Include(s => s.IdExameNavigation)
                .Where(s => s.IdProntuario == idProntuario)
                .OrderByDescending(s => s.DataSolicitacao)
                .ToListAsync();
        }
    }
}