using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class ProntuarioRepository : GenericRepository<Prontuario>, IProntuarioRepository
    {
        public ProntuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Prontuario?> ObterComDetalhesAsync(int idProntuario)
        {
            return await _context.Set<Prontuario>()
                .Include(p => p.IdPacienteNavigation)
                .Include(p => p.IdMedicoNavigation)
                .FirstOrDefaultAsync(p => p.IdProntuario == idProntuario);
        }

        public async Task<Prontuario?> ObterAtivoPorPacienteAsync(int idPaciente)
        {
            return await _context.Set<Prontuario>()
                .FirstOrDefaultAsync(p => p.IdPaciente == idPaciente && p.StatusProntuario == "ATIVO");
        }
    }
}