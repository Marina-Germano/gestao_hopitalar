using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class TriagemService : ITriagemService
    {
        private readonly ITriagemRepository _triagemRepository;
        private readonly IGenericRepository<Pessoa> _pessoaRepository;
        private readonly IGenericRepository<Paciente> _pacienteRepository;

        public TriagemService(
            ITriagemRepository triagemRepository,
            IGenericRepository<Pessoa> pessoaRepository,
            IGenericRepository<Paciente> pacienteRepository,
            IGenericRepository<Usuario> usuarioRepository)
        {
            _triagemRepository = triagemRepository;
            _pessoaRepository = pessoaRepository;
            _pacienteRepository = pacienteRepository;
        }

        public async Task<int> CadastrarAsync(TriagemCadastroDto dto, string loginUsuarioLogado)
        {
            // 1. Busca o Paciente pelo CPF
            var pessoas = await _pessoaRepository.FindAsync(p => p.Cpf == dto.CpfPaciente);
            var pessoa = pessoas.FirstOrDefault();
            if (pessoa == null) throw new Exception($"Nenhum paciente encontrado com o CPF: {dto.CpfPaciente}");

            var pacientes = await _pacienteRepository.FindAsync(p => p.IdPessoa == pessoa.IdPessoa);
            var paciente = pacientes.FirstOrDefault();
            if (paciente == null) throw new Exception("O CPF informado não pertence a um paciente ativo.");

            // 3. Monta a Triagem
            var triagem = new Triagem
            {
                IdPaciente = paciente.IdPaciente,
                Pressao = dto.PressaoArterial,
                Temperatura = dto.Temperatura,
                FrequenciaCardiaca = dto.FrequenciaCardiaca,
                Saturacao = dto.Saturacao,
                Queixa = dto.QueixaPrincipal,
                EscalaDor = dto.EscalaDor,
                Observacoes = dto.Observacoes,
                Risco = dto.ClassificacaoRisco,
                Internacao = "NAO" // Inicialmente a triagem não está internada
            };

            await _triagemRepository.AddAsync(triagem);
            await _triagemRepository.SaveChangesAsync();

            return triagem.IdTriagem; // 🟢 Retorna o ID gerado para ser lido no Flutter
        }

        public async Task<TriagemCadastroDto?> ObterParaEdicaoAsync(int id)
        {
            var triagem = await _triagemRepository.ObterComDetalhesAsync(id);
            if (triagem == null || triagem.IdPacienteNavigation?.IdPessoaNavigation == null) return null;

            return new TriagemCadastroDto
            {
                CpfPaciente = triagem.IdPacienteNavigation.IdPessoaNavigation.Cpf,
                PressaoArterial = triagem.Pressao ?? "",
                Temperatura = triagem.Temperatura ?? 0m,
                FrequenciaCardiaca = triagem.FrequenciaCardiaca ?? 0,
                Saturacao = triagem.Saturacao ?? 0,
                QueixaPrincipal = triagem.Queixa ?? "",
                EscalaDor = triagem.EscalaDor ?? 0,
                Observacoes = triagem.Observacoes,
                ClassificacaoRisco = triagem.Risco ?? ""
            };
        }

        public async Task AtualizarAsync(int id, TriagemCadastroDto dto)
        {
            var triagem = await _triagemRepository.ObterComDetalhesAsync(id);
            if (triagem == null) throw new Exception("Triagem não encontrada.");

            triagem.Pressao = dto.PressaoArterial;
            triagem.Temperatura = dto.Temperatura;
            triagem.FrequenciaCardiaca = dto.FrequenciaCardiaca;
            triagem.Saturacao = dto.Saturacao;
            triagem.Queixa = dto.QueixaPrincipal;
            triagem.EscalaDor = dto.EscalaDor;
            triagem.Observacoes = dto.Observacoes;
            triagem.Risco = dto.ClassificacaoRisco;

            _triagemRepository.Update(triagem);
            await _triagemRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TriagemListagemDto>> ObterTodasAsync()
        {
            var triagens = await _triagemRepository.ObterTodasComDetalhesAsync();
            
            return triagens.Select(t => new TriagemListagemDto
            {
                IdTriagem = t.IdTriagem,
                IdPaciente = t.IdPaciente,
                NomePaciente = t.IdPacienteNavigation?.IdPessoaNavigation?.Nome ?? "N/A",
                CpfPaciente = t.IdPacienteNavigation?.IdPessoaNavigation?.Cpf ?? "N/A",
                ClassificacaoRisco = t.Risco ?? "Não Classificado",
            });
        }

        public async Task ExcluirAsync(int id)
        {
            var triagem = await _triagemRepository.GetByIdAsync(id);
            if (triagem != null)
            {
                _triagemRepository.Delete(triagem);
                await _triagemRepository.SaveChangesAsync();
            }
        }

    }
}