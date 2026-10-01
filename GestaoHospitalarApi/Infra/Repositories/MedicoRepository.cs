using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class MedicoRepository : GenericRepository<Medico>, IMedicoRepository
    {
        public MedicoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Medico?> ObterComDetalhesAsync(int id)
        {
            return await _context.Set<Medico>()
                .Include(m => m.IdEspecialidadeNavigation)
                .Include(m => m.IdUsuarioNavigation) // Busca o Usuário
                    .ThenInclude(u => u.IdPessoaNavigation) // E através do Usuário, busca a Pessoa
                .FirstOrDefaultAsync(m => m.IdMedico == id);
        }

        public async Task<IEnumerable<Medico>> ObterTodosComPessoaEEspecialidadeAsync(int ativo)
        {
            return await _context.Set<Medico>()
                .Include(m => m.IdEspecialidadeNavigation)
                .Include(m => m.IdUsuarioNavigation)
                    .ThenInclude(u => u.IdPessoaNavigation)
                .Where(m => m.IdUsuarioNavigation != null && m.IdUsuarioNavigation.Ativo == ativo) // Checa o Ativo no Usuário!
                .ToListAsync();
        }
    }
}