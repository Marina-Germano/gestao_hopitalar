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
        //private readonly IGenericRepository<Usuario> _usuarioRepository;

        public TriagemService(
            ITriagemRepository triagemRepository,
            IGenericRepository<Pessoa> pessoaRepository,
            IGenericRepository<Paciente> pacienteRepository,
            IGenericRepository<Usuario> usuarioRepository)
        {
            _triagemRepository = triagemRepository;
            _pessoaRepository = pessoaRepository;
            _pacienteRepository = pacienteRepository;
            //_usuarioRepository = usuarioRepository;
        }

        public async Task CadastrarAsync(TriagemCadastroDto dto, string loginUsuarioLogado)
        {
            // 1. Busca o Paciente pelo CPF
            var pessoas = await _pessoaRepository.FindAsync(p => p.Cpf == dto.CpfPaciente);
            var pessoa = pessoas.FirstOrDefault();
            if (pessoa == null) throw new Exception("Nenhum paciente encontrado com este CPF.");

            var pacientes = await _pacienteRepository.FindAsync(p => p.IdPessoa == pessoa.IdPessoa);
            var paciente = pacientes.FirstOrDefault();
            if (paciente == null) throw new Exception("O CPF informado não pertence a um paciente ativo.");

            // 2. Busca o Usuário Responsável pelo Login (Token JWT)
            // var usuarios = await _usuarioRepository.FindAsync(u => u.Login == loginUsuarioLogado);
            // var usuarioLogado = usuarios.FirstOrDefault();
            // if (usuarioLogado == null) throw new Exception("Usuário responsável não encontrado ou sessão inválida.");

            // 3. Monta a Triagem
            var triagem = new Triagem
            {
                IdPaciente = paciente.IdPaciente,
                //IdUsuario = usuarioLogado.IdUsuario,
                //DataTriagem = DateTime.Now, // Data e hora cravadas pelo servidor automaticamente
                
                Pressao = dto.PressaoArterial,
                Temperatura = dto.Temperatura,
                FrequenciaCardiaca = dto.FrequenciaCardiaca,
                Saturacao = dto.Saturacao,
                
                Queixa = dto.QueixaPrincipal,
                EscalaDor = dto.EscalaDor,
                //Alergias = dto.AlergiasRelatadas,
                Observacoes = dto.Observacoes,
                Risco = dto.ClassificacaoRisco
            };

            await _triagemRepository.AddAsync(triagem);
            await _triagemRepository.SaveChangesAsync();
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
                //AlergiasRelatadas = triagem.AlergiasRelatadas,
                Observacoes = triagem.Observacoes,
                ClassificacaoRisco = triagem.Risco ?? ""
            };
        }

        public async Task AtualizarAsync(int id, TriagemCadastroDto dto)
        {
            var triagem = await _triagemRepository.ObterComDetalhesAsync(id);
            if (triagem == null) throw new Exception("Triagem não encontrada.");

            // Triagem normalmente não muda de paciente, mas se CPF mudar, precisaria buscar novamente.
            // Para manter a segurança, vamos atualizar apenas os dados clínicos:
            triagem.Pressao = dto.PressaoArterial;
            triagem.Temperatura = dto.Temperatura;
            triagem.FrequenciaCardiaca = dto.FrequenciaCardiaca;
            triagem.Saturacao = dto.Saturacao;
            triagem.Queixa = dto.QueixaPrincipal;
            triagem.EscalaDor = dto.EscalaDor;
            //triagem.AlergiasRelatadas = dto.AlergiasRelatadas;
            triagem.Observacoes = dto.Observacoes;
            triagem.Risco = dto.ClassificacaoRisco;
            // A Data da Triagem e o Responsável original não são alterados em edições.

            _triagemRepository.Update(triagem);
            await _triagemRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TriagemListagemDto>> ObterTodasAsync()
        {
            var triagens = await _triagemRepository.ObterTodasComDetalhesAsync();
            
            return triagens.Select(t => new TriagemListagemDto
            {
                IdTriagem = t.IdTriagem,
                NomePaciente = t.IdPacienteNavigation?.IdPessoaNavigation?.Nome ?? "N/A",
                CpfPaciente = t.IdPacienteNavigation?.IdPessoaNavigation?.Cpf ?? "N/A",
                //DataTriagem = t.DataTriagem ?? DateTime.MinValue,
                ClassificacaoRisco = t.Risco ?? "Não Classificado",
                //EnfermeiroResponsavel = t.IdUsuarioNavigation?.IdPessoaNavigation?.Nome ?? "N/A"
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