using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class InternacaoRepository : GenericRepository<Internacao>, IInternacaoRepository
    {
        public InternacaoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Internacao>> ObterTodasComDetalhesAsync()
        {
            return await _context.Set<Internacao>()
                .Include(i => i.IdLeitoNavigation)
                .Include(i => i.IdProntuarioNavigation)
                    .ThenInclude(p => p.IdPacienteNavigation)
                .OrderByDescending(i => i.DataEntrada)
                .ToListAsync();
        }

        public async Task<Internacao?> ObterComDetalhesFaturamentoAsync(int idInternacao)
        {
            return await _context.Set<Internacao>()
                .Include(i => i.IdLeitoNavigation)
                .Include(i => i.IdProntuarioNavigation)
                    .ThenInclude(p => p.IdMedicoNavigation)
                .Include(i => i.IdProntuarioNavigation)
                    .ThenInclude(p => p.IdPacienteNavigation)
                        .ThenInclude(pac => pac.PacienteConvenios) // Acessa a tabela intermediária
                            .ThenInclude(pc => pc.IdConvenioNavigation) // Busca os dados do convênio
                .FirstOrDefaultAsync(i => i.IdInternacao == idInternacao);
        }
    }
}