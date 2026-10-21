using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class LeitoService : ILeitoService
    {
        private readonly ILeitoRepository _leitoRepository;

        public LeitoService(ILeitoRepository leitoRepository)
        {
            _leitoRepository = leitoRepository;
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
            var leitos = await _leitoRepository.ObterLeitosVagosAsync();
            return leitos.Select(l => new LeitoListagemDto
            {
                IdLeito = l.IdLeito,
                Numero = l.Numero ?? "",
                Situacao = l.Situacao ?? "Vago",
                IdAla = l.IdAla,
                NomeAla = l.IdAlaNavigation?.NomeAla ?? "",   // Verifique se no seu modelo Ala é Nome ou Descricao
                AndarAla = l.IdAlaNavigation?.Andar ?? ""
            });
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