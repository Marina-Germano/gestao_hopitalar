using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class AgendamentoRepository : GenericRepository<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Agendamento>> ObterTodosComDetalhesAsync()
        {
            return await _context.Set<Agendamento>()
                .Include(a => a.IdPacienteNavigation)
                .Include(a => a.IdMedicoNavigation)
                    .ThenInclude(m => m.IdUsuarioNavigation)
                        .ThenInclude(u => u.IdPessoaNavigation)
                .Include(a => a.IdSalaNavigation)
                .OrderBy(a => a.DataHora)
                .ToListAsync();
        }
    }
}