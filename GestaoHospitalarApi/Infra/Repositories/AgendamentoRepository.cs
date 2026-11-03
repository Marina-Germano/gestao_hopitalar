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

    public async Task<IEnumerable<Agendamento>> ObterPorMesAnoAsync(int ano, int mes)
        {
            return await _context.Agendamentos
                .Include(a => a.IdPacienteNavigation)
                .Include(a => a.IdMedicoNavigation)
                .Include(a => a.IdSalaNavigation)
                .Where(a => a.DataHora.Year == ano && a.DataHora.Month == mes)
                .OrderBy(a => a.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> ObterPorDataAsync(DateTime data)
        {
            return await _context.Agendamentos
                .Include(a => a.IdPacienteNavigation)
                .Include(a => a.IdMedicoNavigation)
                .Include(a => a.IdSalaNavigation)
                .Where(a => a.DataHora.Date == data.Date)
                .OrderBy(a => a.DataHora)
                .ToListAsync();
        }

        public async Task<Agendamento?> ObterPorIdComDetalhesAsync(int idAgendamento)
        {
            return await _context.Agendamentos
                .Include(a => a.IdPacienteNavigation)
                .Include(a => a.IdMedicoNavigation)
                .Include(a => a.IdSalaNavigation)
                .FirstOrDefaultAsync(a => a.IdAgendamento == idAgendamento);
        }

        public async Task<bool> AtualizarStatusAsync(int idAgendamento, string novoStatus)
        {
            var agendamento = await _context.Agendamentos.FindAsync(idAgendamento);
            if (agendamento == null) return false;

            agendamento.Status = novoStatus.ToUpper();
            _context.Agendamentos.Update(agendamento);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}