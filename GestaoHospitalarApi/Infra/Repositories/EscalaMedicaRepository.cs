using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class EscalaMedicaRepository : GenericRepository<EscalaMedica>, IEscalaMedicaRepository
    {
        private readonly AppDbContext _context;

        public EscalaMedicaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EscalaMedica>> ObterPorMesAnoAsync(int ano, int mes)
        {
            return await _context.EscalaMedicas
                .Where(e => e.DataEscala.Year == ano && e.DataEscala.Month == mes)
                .OrderBy(e => e.DataEscala)
                .ThenBy(e => e.HoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<EscalaMedica>> ObterTodasComMedicoAsync()
        {
            return await _context.EscalaMedicas
                .OrderBy(e => e.DataEscala)
                .ToListAsync();
        }

        public async Task<EscalaMedica?> ObterPorIdComMedicoAsync(int idEscala)
        {
            return await _context.EscalaMedicas
                .FirstOrDefaultAsync(e => e.IdEscala == idEscala);
        }

        public async Task<int> CadastrarAsync(EscalaMedica escala)
        {
            _context.EscalaMedicas.Add(escala);
            await _context.SaveChangesAsync();
            return escala.IdEscala;
        }

        public async Task<bool> AtualizarAsync(EscalaMedica escala)
        {
            _context.EscalaMedicas.Update(escala);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExcluirAsync(int idEscala)
        {
            var escala = await _context.EscalaMedicas.FindAsync(idEscala);
            if (escala == null) return false;

            _context.EscalaMedicas.Remove(escala);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}