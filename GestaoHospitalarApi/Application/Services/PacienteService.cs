using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class PacienteService : IPacienteService
    {
        // Usaremos um IPacienteRepository (que você criará na Domain) para poder usar Includes nas buscas
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IGenericRepository<Convenio> _convenioRepository;
        private readonly IGenericRepository<PacienteConvenio> _pacienteConvenioRepository;

        public PacienteService(
            IPacienteRepository pacienteRepository,
            IGenericRepository<Convenio> convenioRepository,
            IGenericRepository<PacienteConvenio> pacienteConvenioRepository)
        {
            _pacienteRepository = pacienteRepository;
            _convenioRepository = convenioRepository;
            _pacienteConvenioRepository = pacienteConvenioRepository;
        }

        public async Task CadastrarAsync(PacienteCadastroDto dto)
        {
            // 1. Instancia a Pessoa
            var pessoa = new Pessoa
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                Telefone = dto.Telefone,
                Nascimento = DateOnly.FromDateTime(dto.DataNascimento),
                Sexo = dto.Sexo,
                Cep = dto.Cep,
                Rua = dto.Rua,
                NumeroCasa = dto.Numero,
                Bairro = dto.Bairro?? string.Empty,
                Cidade = dto.Cidade?? string.Empty,
                Estado = dto.Estado?? string.Empty
            };

            // 2. Instancia o Paciente (vinculando a pessoa)
            var paciente = new Paciente
            {
                Alergias = dto.Alergias?? string.Empty,
                TipoSanguineo = dto.TipoSanguineo?? string.Empty,
                HistoricoClinico = dto.HistoricoClinico?? string.Empty,
                NomeResponsavel = dto.NomeResponsavel?? string.Empty,
                Ativo = 1, // 1 = Ativo, 0 = Inativo/Arquivado
                IdPessoaNavigation = pessoa
            };

            await _pacienteRepository.AddAsync(paciente);
            await _pacienteRepository.SaveChangesAsync();

            // 3. Trata o Convênio, se houver
            if (dto.Convenio != null && !string.IsNullOrWhiteSpace(dto.Convenio.NomeConvenio))
            {
                var convenios = await _convenioRepository.FindAsync(c => c.NomeConvenio == dto.Convenio.NomeConvenio);
                var convenioDb = convenios.FirstOrDefault();

                if (convenioDb != null)
                {
                    var pacienteConvenio = new PacienteConvenio
                    {
                        IdPaciente = paciente.IdPaciente,
                        IdConvenio = convenioDb.IdConvenio,
                        NumeroCarteira = dto.Convenio.Numero,
                        Validade = DateOnly.FromDateTime(dto.Convenio.Validade)
                    };
                    await _pacienteConvenioRepository.AddAsync(pacienteConvenio);
                    await _pacienteConvenioRepository.SaveChangesAsync();
                }
            }
        }

        public async Task<PacienteCadastroDto?> ObterParaEdicaoAsync(int id)
        {
            // O repositório precisa trazer o paciente + pessoa + paciente_convenio + convenio
            var paciente = await _pacienteRepository.ObterComDetalhesAsync(id);
            if (paciente == null) return null;

            var dto = new PacienteCadastroDto
            {
                // Dados Pessoa
                Nome = paciente.IdPessoaNavigation.Nome,
                Cpf = paciente.IdPessoaNavigation.Cpf,
                Telefone = paciente.IdPessoaNavigation.Telefone,
                DataNascimento = paciente.IdPessoaNavigation.Nascimento?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                Sexo = paciente.IdPessoaNavigation.Sexo ?? "",
                Cep = paciente.IdPessoaNavigation.Cep ?? "",
                Rua = paciente.IdPessoaNavigation.Rua ?? string.Empty,
                Numero = paciente.IdPessoaNavigation.NumeroCasa ?? string.Empty,
                Bairro = paciente.IdPessoaNavigation.Bairro ?? string.Empty,
                Cidade = paciente.IdPessoaNavigation.Cidade ?? string.Empty,
                Estado = paciente.IdPessoaNavigation.Estado ?? string.Empty,

                // Dados Paciente
                Alergias = paciente.Alergias ?? "",
                TipoSanguineo = paciente.TipoSanguineo ?? "",
                HistoricoClinico = paciente.HistoricoClinico ?? "",
                NomeResponsavel = paciente.NomeResponsavel ?? ""
            };

            // Se tiver convênio vinculado
            var pacienteConvenio = paciente.PacienteConvenios?.FirstOrDefault();
            if (pacienteConvenio != null && pacienteConvenio.IdConvenioNavigation != null)
            {
                dto.Convenio = new PacienteConvenioCadastroDto
                {
                    NomeConvenio = pacienteConvenio.IdConvenioNavigation.NomeConvenio,
                    Numero = pacienteConvenio.NumeroCarteira ?? "",
                    Validade = pacienteConvenio.Validade.ToDateTime(TimeOnly.MinValue)
                };
            }

            return dto;
        }

        public async Task AtualizarAsync(int id, PacienteCadastroDto dto)
        {
            var paciente = await _pacienteRepository.ObterComDetalhesAsync(id);
            if (paciente == null) throw new Exception("Paciente não encontrado.");

            // Atualiza Dados da Pessoa
            paciente.IdPessoaNavigation.Nome = dto.Nome;
            paciente.IdPessoaNavigation.Cpf = dto.Cpf;
            paciente.IdPessoaNavigation.Telefone = dto.Telefone;
            paciente.IdPessoaNavigation.Nascimento = DateOnly.FromDateTime(dto.DataNascimento);
            paciente.IdPessoaNavigation.Sexo = dto.Sexo;
            paciente.IdPessoaNavigation.Cep = dto.Cep;
            paciente.IdPessoaNavigation.Rua = dto.Rua;
            paciente.IdPessoaNavigation.NumeroCasa = dto.Numero;
            paciente.IdPessoaNavigation.Bairro = dto.Bairro;
            paciente.IdPessoaNavigation.Cidade = dto.Cidade;
            paciente.IdPessoaNavigation.Estado = dto.Estado;

            // Atualiza Dados do Paciente
            paciente.Alergias = dto.Alergias;
            paciente.TipoSanguineo = dto.TipoSanguineo;
            paciente.HistoricoClinico = dto.HistoricoClinico;
            paciente.NomeResponsavel = dto.NomeResponsavel;

            _pacienteRepository.Update(paciente);
            
            // Lógica de atualização do Convênio
            var pcExistente = paciente.PacienteConvenios?.FirstOrDefault(); // Proteção contra nulo adicionada aqui!

            if (dto.Convenio != null && !string.IsNullOrWhiteSpace(dto.Convenio.NomeConvenio))
            {
                var convenios = await _convenioRepository.FindAsync(c => c.NomeConvenio == dto.Convenio.NomeConvenio);
                var convenioDb = convenios.FirstOrDefault();

                if (convenioDb != null)
                {
                    if (pcExistente != null)
                    {
                        // Atualiza o existente
                        pcExistente.IdConvenio = convenioDb.IdConvenio;
                        pcExistente.NumeroCarteira = dto.Convenio.Numero;
                        pcExistente.Validade = DateOnly.FromDateTime(dto.Convenio.Validade);
                        _pacienteConvenioRepository.Update(pcExistente);
                    }
                    else
                    {
                        // Cria um novo vínculo
                        var novoPc = new PacienteConvenio
                        {
                            IdPaciente = paciente.IdPaciente,
                            IdConvenio = convenioDb.IdConvenio,
                            NumeroCarteira = dto.Convenio.Numero,
                            Validade = DateOnly.FromDateTime(dto.Convenio.Validade)
                        };
                        await _pacienteConvenioRepository.AddAsync(novoPc);
                    }
                }
            }
            else
            {
                // REGRA NOVA: Se o front mandou convênio nulo, mas o paciente tinha um, nós removemos (virou Particular)
                if (pcExistente != null)
                {
                    _pacienteConvenioRepository.Delete(pcExistente);
                }
            }

            await _pacienteRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<PacienteListagemDto>> ObterTodosAtivosAsync()
        {
            var pacientes = await _pacienteRepository.ObterTodosComPessoaAsync(ativo: 1);
            return pacientes.Select(p => new PacienteListagemDto
            {
                Id = p.IdPaciente,
                Nome = p.IdPessoaNavigation!.Nome,
                Cpf = p.IdPessoaNavigation.Cpf
            });
        }

        public async Task<IEnumerable<PacienteListagemDto>> ObterTodosInativosAsync()
        {
            var pacientes = await _pacienteRepository.ObterTodosComPessoaAsync(ativo: 0);
            return pacientes.Select(p => new PacienteListagemDto
            {
                Id = p.IdPaciente,
                Nome = p.IdPessoaNavigation!.Nome,
                Cpf = p.IdPessoaNavigation!.Cpf
            });
        }

        public async Task ArquivarAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);
            if (paciente != null)
            {
                paciente.Ativo = 0; // Inativa o paciente
                _pacienteRepository.Update(paciente);
                await _pacienteRepository.SaveChangesAsync();
            }
        }

        public async Task AtivarAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);
            if (paciente != null)
            {
                paciente.Ativo = 1; // Reativa o paciente
                _pacienteRepository.Update(paciente);
                await _pacienteRepository.SaveChangesAsync();
            }
        }

        public async Task ExcluirAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);
            if (paciente != null)
            {
                _pacienteRepository.Delete(paciente);
                await _pacienteRepository.SaveChangesAsync();
            }
        }
    }
}