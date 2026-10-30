using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class PacienteConvenioRepository : GenericRepository<PacienteConvenio>, IPacienteConvenioRepository
    {
        public PacienteConvenioRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<PacienteConvenio>> ObterPorPacienteAsync(int idPaciente)
        {
            return await _context.Set<PacienteConvenio>()
                .Include(pc => pc.IdPacienteNavigation)
                .Include(pc => pc.IdConvenioNavigation)
                .Where(pc => pc.IdPaciente == idPaciente)
                .ToListAsync();
        }
    }
}