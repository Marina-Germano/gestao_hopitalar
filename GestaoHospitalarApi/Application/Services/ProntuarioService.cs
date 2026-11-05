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
        private readonly ITriagemRepository _triagemRepository;

        public ProntuarioService(
            IProntuarioRepository prontuarioRepository,
            IGenericRepository<Internacao> internacaoRepository,
            IGenericRepository<Leito> leitoRepository,
            ITriagemRepository triagemRepository)
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
                RiscoEvasao = dto.RiscoEvasao?.ToUpper() switch
                {
                    "BAIXO" => "BAIXO",
                    "MÉDIO" or "MEDIO" => "MEDIO",
                    "ALTO" => "ALTO",
                    _ => "BAIXO"
                },
                Evolucao = dto.Evolucao,
                StatusProntuario = dto.StatusProntuario,
                DataAbertura = DateTime.Now
            };

            await _prontuarioRepository.AddAsync(prontuario);
            await _prontuarioRepository.SaveChangesAsync();
            return prontuario.IdProntuario;
        }

        public async Task<int> EncaminharTriagemAsync(int idTriagem, TriagemEncaminhamentoDto dto)
        {
            var triagem = await _triagemRepository.ObterComDetalhesAsync(idTriagem);
            if (triagem == null) throw new Exception($"Triagem com ID [{idTriagem}] não encontrada.");

            // 1. Cria o Prontuário garantindo a conversão do RiscoEvasao
            var prontuario = new Prontuario
            {
                IdPaciente = triagem.IdPaciente,
                IdTriagem = idTriagem,
                IdMedico = dto.IdMedicoResponsavel,
                IdSala = dto.TipoDestino == "SALA" ? dto.IdSala : null,
                RiscoEvasao = dto.RiscoEvasao?.ToUpper() switch
                {
                    "BAIXO" => "baixo",
                    "MÉDIO" or "MEDIO" => "médio",
                    "ALTO" => "alto",
                    _ => "baixo"
                },
                Isolamento = dto.PrecisaIsolamento ? "SIM" : "NAO",
                StatusProntuario = "ATIVO",
                DataAbertura = DateTime.Now
            };
            await _prontuarioRepository.AddAsync(prontuario);

            // 🟢 PRIMEIRO SAVE: Salva o Prontuário para gerar o IdProntuario no banco de dados
            await _prontuarioRepository.SaveChangesAsync();

            // 2. Se for LEITO, cria a Internação e Ocupa o Leito
            if (dto.TipoDestino == "LEITO" && dto.IdLeito.HasValue)
            {
                var internacao = new Internacao
                {
                    IdProntuario = prontuario.IdProntuario, // Agora tem o ID real gerado pelo banco
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

            // 3. Atualiza Triagem (Remove o paciente da fila)
            triagem.Internacao = "SIM";
            _triagemRepository.Update(triagem);

            // 🟢 SEGUNDO SAVE: Salva a Internação, Leito e Triagem com a garantia de que o Prontuário já existe
            await _prontuarioRepository.SaveChangesAsync();

            return prontuario.IdProntuario;
        }
    }
}