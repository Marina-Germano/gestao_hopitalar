using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class ProntuarioService : IProntuarioService
    {
        private readonly IProntuarioRepository _prontuarioRepository;
        private readonly IGenericRepository<Internacao> _internacaoRepository;
        private readonly IGenericRepository<Leito> _leitoRepository;
        private readonly ITriagemRepository _triagemRepository; // 🟢 Alterado de IGeneric para ITriagemRepository

        public ProntuarioService(
            IProntuarioRepository prontuarioRepository,
            IGenericRepository<Internacao> internacaoRepository,
            IGenericRepository<Leito> leitoRepository,
            ITriagemRepository triagemRepository) // 🟢 Alterado aqui
        {
            _prontuarioRepository = prontuarioRepository;
            _internacaoRepository = internacaoRepository;
            _leitoRepository = leitoRepository;
            _triagemRepository = triagemRepository;
        }

        public async Task<int> CadastrarAsync(ProntuarioCadastroDto dto)
        {
            var prontuario = new Prontuario
            {
                IdPaciente = dto.IdPaciente,
                IdTriagem = dto.IdTriagem,
                IdMedico = dto.IdMedico,
                IdSala = dto.IdSala,
                RiscoEvasao = dto.RiscoEvasao,
                Evolucao = dto.Evolucao,
                StatusProntuario = dto.StatusProntuario,
                DataAbertura = DateTime.Now
            };

            await _prontuarioRepository.AddAsync(prontuario);
            return prontuario.IdProntuario;
        }

        public async Task<int> EncaminharTriagemAsync(int idTriagem, TriagemEncaminhamentoDto dto)
        {
            // 🟢 Usando ObterComDetalhesAsync e logando qual ID exatamente falhou
            var triagem = await _triagemRepository.ObterComDetalhesAsync(idTriagem);
            if (triagem == null) throw new Exception($"Triagem com ID [{idTriagem}] não encontrada.");

            // 1. Cria o Prontuário
            var prontuario = new Prontuario
            {
                IdPaciente = triagem.IdPaciente,
                IdTriagem = idTriagem,
                IdMedico = dto.IdMedicoResponsavel,
                IdSala = dto.TipoDestino == "SALA" ? dto.IdSala : null,
                RiscoEvasao = dto.RiscoEvasao,
                StatusProntuario = "ATIVO",
                DataAbertura = DateTime.Now
            };
            await _prontuarioRepository.AddAsync(prontuario);

            // 2. Se for LEITO, cria a Internação e Ocupa o Leito
            if (dto.TipoDestino == "LEITO" && dto.IdLeito.HasValue)
            {
                var internacao = new Internacao
                {
                    IdProntuario = prontuario.IdProntuario,
                    IdLeito = dto.IdLeito.Value,
                    DataEntrada = DateTime.Now,
                    StatusInternacao = "ATIVA"
                };
                await _internacaoRepository.AddAsync(internacao);

                var leito = await _leitoRepository.GetByIdAsync(dto.IdLeito.Value);
                if (leito != null)
                {
                    leito.Situacao = "OCUPADO"; 
                    _leitoRepository.Update(leito);
                }
            }

            // 3. Atualiza Triagem
            triagem.Internacao = "SIM";
            _triagemRepository.Update(triagem);

            return prontuario.IdProntuario;
        }
    }
}