using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class InternacaoService : IInternacaoService
    {
        private readonly IInternacaoRepository _internacaoRepository;
        private readonly IGenericRepository<Leito> _leitoRepository;
        private readonly IGenericRepository<Prontuario> _prontuarioRepository;
        private readonly IFaturamentoService _faturamentoService; // Injeção do Faturamento!

        public InternacaoService(
            IInternacaoRepository internacaoRepository, 
            IGenericRepository<Leito> leitoRepository,
            IGenericRepository<Prontuario> prontuarioRepository,
            IFaturamentoService faturamentoService)
        {
            _internacaoRepository = internacaoRepository;
            _leitoRepository = leitoRepository;
            _prontuarioRepository = prontuarioRepository;
            _faturamentoService = faturamentoService;
        }

        public async Task CadastrarAsync(InternacaoCadastroDto dto)
        {
            var internacao = new Internacao
            {
                IdProntuario = dto.IdProntuario,
                IdLeito = dto.IdLeito,
                DataEntrada = DateTime.Now,
                StatusInternacao = "ATIVA"
            };

            // Regra: Ocupar o leito
            var leito = await _leitoRepository.GetByIdAsync(dto.IdLeito);
            if (leito != null)
            {
                leito.Situacao = "OCUPADO";
                _leitoRepository.Update(leito);
            }

            await _internacaoRepository.AddAsync(internacao);
            await _internacaoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<InternacaoListagemDto>> ObterTodasAsync()
        {
            var internacoes = await _internacaoRepository.ObterTodasComDetalhesAsync();
            return internacoes.Select(i => new InternacaoListagemDto
            {
                IdInternacao = i.IdInternacao,
                NomePaciente = i.IdProntuarioNavigation?.IdPacienteNavigation?.IdPessoaNavigation?.Nome ?? "",
                NumeroLeito = i.IdLeitoNavigation?.Numero ?? "",
                DataEntrada = i.DataEntrada,
                DataAlta = i.DataAlta,
                StatusInternacao = i.StatusInternacao ?? ""
            });
        }

        public async Task DarAltaAsync(int idInternacao)
        {
            var internacao = await _internacaoRepository.GetByIdAsync(idInternacao);
            if (internacao == null) throw new Exception("Internação não encontrada.");

            internacao.StatusInternacao = "ALTA";
            internacao.DataAlta = DateTime.Now;
            _internacaoRepository.Update(internacao);

            // Regra: Enviar leito para higienização
            var leito = await _leitoRepository.GetByIdAsync(internacao.IdLeito);
            if (leito != null)
            {
                leito.Situacao = "HIGIENIZACAO";
                _leitoRepository.Update(leito);
            }

            // Regra: Arquivar prontuário
            var prontuario = await _prontuarioRepository.GetByIdAsync(internacao.IdProntuario);
            if (prontuario != null)
            {
                prontuario.StatusProntuario = "ARQUIVADO";
                _prontuarioRepository.Update(prontuario);
            }

            await _internacaoRepository.SaveChangesAsync(); // Salva as alterações da alta

            // Regra Final: Gera o faturamento da conta do paciente
            await _faturamentoService.GerarFaturamentoAltaAsync(idInternacao);
        }
    }
}