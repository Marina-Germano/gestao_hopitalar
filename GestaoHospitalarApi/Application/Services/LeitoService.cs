using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Infra.EF;
using Microsoft.EntityFrameworkCore;

namespace GestaoHospitalarApi.Application.Services
{
    public class LeitoService : ILeitoService
    {
        private readonly ILeitoRepository _leitoRepository;
        private readonly AppDbContext _context;

        public LeitoService(ILeitoRepository leitoRepository, AppDbContext context)
        {
            _leitoRepository = leitoRepository;
            _context = context;
        }

        public async Task CadastrarAsync(LeitoCadastroDto dto)
        {
            var leito = new Leito
            {
                Numero = dto.Numero,
                IdAla = dto.IdAla,
                Situacao = "Vago" // Todo leito novo nasce Vago!
            };

            await _leitoRepository.AddAsync(leito);
            await _leitoRepository.SaveChangesAsync();
        }

        public async Task AtualizarAsync(int id, LeitoCadastroDto dto)
        {
            var leito = await _leitoRepository.GetByIdAsync(id);
            if (leito == null) throw new Exception("Leito não encontrado.");

            leito.Numero = dto.Numero;
            leito.IdAla = dto.IdAla;

            _leitoRepository.Update(leito);
            await _leitoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<LeitoListagemDto>> ObterTodosAsync()
        {
            // 🟢 Busca TODOS os leitos, sem filtrar apenas os vagos, trazendo a árvore completa
            var leitos = await _context.Leitos
                .Include(l => l.IdAlaNavigation)
                .Include(l => l.Internacaos)
                    .ThenInclude(i => i.IdProntuarioNavigation)
                        .ThenInclude(p => p.IdPacienteNavigation)
                            .ThenInclude(pac => pac.IdPessoaNavigation)
                .OrderBy(l => l.IdAlaNavigation.NomeAla)
                .ThenBy(l => l.Numero)
                .ToListAsync();

            return leitos.Select(l => new LeitoListagemDto
            {
                IdLeito = l.IdLeito,
                Numero = l.Numero ?? "",
                Situacao = l.Situacao ?? "VAGO", // Pode vir VAGO, OCUPADO ou HIGIENIZACAO
                IdAla = l.IdAla,
                NomeAla = l.IdAlaNavigation?.NomeAla ?? "",
                AndarAla = l.IdAlaNavigation?.Andar ?? "",
                // 🟢 Proteção com ?. captura o nome se estiver ocupado
                NomePaciente = l.Internacaos?
                    .Where(i => i != null && i.StatusInternacao == "ATIVA")
                    .Select(i => i.IdProntuarioNavigation?.IdPacienteNavigation?.IdPessoaNavigation?.Nome)
                    .FirstOrDefault() ?? ""
            }).ToList();
        }

        public async Task<IEnumerable<LeitoListagemDto>> ObterVagosAsync()
        {
            var leitos = await _leitoRepository.ObterLeitosVagosAsync();
            return leitos.Select(l => new LeitoListagemDto
            {
                IdLeito = l.IdLeito,
                Numero = l.Numero ?? "",
                Situacao = l.Situacao ?? "Vago",
                IdAla = l.IdAla,
                NomeAla = l.IdAlaNavigation?.NomeAla ?? "",
                AndarAla = l.IdAlaNavigation?.Andar ?? ""
            });
        }

        // --- REGRA DE NEGÓCIO DO ADMIN ---
        public async Task LiberarHigienizacaoAsync(int id)
        {
            var leito = await _leitoRepository.GetByIdAsync(id);
            if (leito == null) throw new Exception("Leito não encontrado.");
            
            if (leito.Situacao != "Em higienização")
                throw new Exception("Esta ação só pode ser realizada em leitos que estão em higienização.");

            leito.Situacao = "Vago";
            _leitoRepository.Update(leito);
            await _leitoRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var leito = await _leitoRepository.GetByIdAsync(id);
            if (leito != null)
            {
                if (leito.Situacao == "Ocupado")
                    throw new Exception("Não é possível excluir um leito ocupado.");

                _leitoRepository.Delete(leito);
                await _leitoRepository.SaveChangesAsync();
            }
        }
    }
}