using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class PrescricaoRepository : GenericRepository<Prescricao>, IPrescricaoRepository
    {
        public PrescricaoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Prescricao>> ObterPorProntuarioAsync(int idProntuario)
        {
            return await _context.Set<Prescricao>()
                .Include(p => p.IdMedicoNavigation)
                    .ThenInclude(m => m.IdUsuarioNavigation)
                        .ThenInclude(u => u.IdPessoaNavigation)
                .Include(p => p.IdMedicamentoNavigation)
                    .ThenInclude(m => m.IdAlmoxarifadoNavigation) // Para pegar o nome do medicamento
                .Where(p => p.IdProntuario == idProntuario)
                .ToListAsync();
        }
    }
}