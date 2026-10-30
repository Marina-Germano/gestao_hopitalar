using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class SolicitacaoExameService : ISolicitacaoExameService
    {
        private readonly ISolicitacaoExameRepository _solicitacaoRepository;

        public SolicitacaoExameService(ISolicitacaoExameRepository solicitacaoRepository)
        {
            _solicitacaoRepository = solicitacaoRepository;
        }

        public async Task CadastrarAsync(SolicitacaoExameCadastroDto dto)
        {
            var solicitacao = new SolicitacaoExame
            {
                IdProntuario = dto.IdProntuario,
                IdExame = dto.IdExame,
                IdMedico = dto.IdMedico,
                DataSolicitacao = DateTime.Now,
                StatusExame = "SOLICITADO"
            };

            await _solicitacaoRepository.AddAsync(solicitacao);
            await _solicitacaoRepository.SaveChangesAsync();
        }

        public async Task LancarResultadoAsync(int idSolicitacao, string resultado)
        {
            var solicitacao = await _solicitacaoRepository.GetByIdAsync(idSolicitacao);
            if (solicitacao == null) throw new Exception("Solicitação de exame não encontrada.");

            solicitacao.Resultado = resultado;
            solicitacao.StatusExame = "CONCLUIDO";

            _solicitacaoRepository.Update(solicitacao);
            await _solicitacaoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<SolicitacaoExameListagemDto>> ObterPorProntuarioAsync(int idProntuario)
        {
            var solicitacoes = await _solicitacaoRepository.ObterPorProntuarioAsync(idProntuario);
            return solicitacoes.Select(s => new SolicitacaoExameListagemDto
            {
                IdSolicitacao = s.IdSolicitacao,
                IdProntuario = s.IdProntuario,
                IdMedico = s.IdMedico,
                NomeMedico = s.IdMedicoNavigation?.IdUsuarioNavigation?.IdPessoaNavigation?.Nome ?? "Médico Desconhecido",
                IdExame = s.IdExame,
                NomeExame = s.IdExameNavigation?.Nome ?? "Exame Desconhecido",
                DataSolicitacao = s.DataSolicitacao,
                StatusExame = s.StatusExame ?? "SOLICITADO",
                Resultado = s.Resultado
            });
        }
    }
}