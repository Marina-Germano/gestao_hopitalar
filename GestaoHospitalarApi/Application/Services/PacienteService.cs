
using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GestaoHospitalarApi.Application.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IGenericRepository<Convenio> _convenioRepository;
        private readonly IGenericRepository<PacienteConvenio> _pacienteConvenioRepository;
        private readonly IMemoryCache _cache;

        private const string CachePacientesAtivos = "pacientes_ativos";
        private const string CachePacientesInativos = "pacientes_inativos";

        public PacienteService(
            IPacienteRepository pacienteRepository,
            IGenericRepository<Convenio> convenioRepository,
            IGenericRepository<PacienteConvenio> pacienteConvenioRepository,
            IMemoryCache cache)
        {
            _pacienteRepository = pacienteRepository;
            _convenioRepository = convenioRepository;
            _pacienteConvenioRepository = pacienteConvenioRepository;
            _cache = cache;
        }

        // INVALIDAR CACHE DE PACIENTES
        private void InvalidarCachePacientes()
        {
            _cache.Remove(CachePacientesAtivos);
            _cache.Remove(CachePacientesInativos);
        }

        // CADASTRAR PACIENTE
        public async Task CadastrarAsync(PacienteCadastroDto dto)
        {
            // 1. Instancia a Pessoa
            var pessoa = new Pessoa
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Nascimento = DateOnly.FromDateTime(dto.DataNascimento),
                Sexo = dto.Sexo,
                Cep = dto.Cep,
                Rua = dto.Rua,
                NumeroCasa = dto.Numero,
                Bairro = dto.Bairro ?? string.Empty,
                Cidade = dto.Cidade ?? string.Empty,
                Estado = dto.Estado ?? string.Empty,
                DataCriacao = DateTime.Now
            };

            // 2. Instancia o Paciente
            var paciente = new Paciente
            {
                Alergias = dto.Alergias ?? string.Empty,
                TipoSanguineo = dto.TipoSanguineo ?? string.Empty,
                HistoricoClinico = dto.HistoricoClinico ?? string.Empty,
                NomeResponsavel = dto.NomeResponsavel ?? string.Empty,
                Ativo = 1,
                IdPessoaNavigation = pessoa
            };

            // 3. Salva Pessoa e Paciente
            try
            {
                await _pacienteRepository.AddAsync(paciente);
                await _pacienteRepository.SaveChangesAsync();

                // O paciente foi salvo. Limpa o cache.
                InvalidarCachePacientes();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"ERRO AO SALVAR PESSOA/PACIENTE: {ex.GetBaseException().Message}", ex);
            }

            // 4. Trata o Convênio, se houver
            if (dto.Convenio != null &&
                !string.IsNullOrWhiteSpace(dto.Convenio.NomeConvenio))
            {
                var convenios = await _convenioRepository.FindAsync(
                    c => c.NomeConvenio == dto.Convenio.NomeConvenio);

                var convenioDb = convenios.FirstOrDefault();

                if (convenioDb == null)
                {
                    throw new Exception(
                        $"Convênio '{dto.Convenio.NomeConvenio}' não encontrado.");
                }

                var pacienteConvenio = new PacienteConvenio
                {
                    IdPaciente = paciente.IdPaciente,
                    IdConvenio = convenioDb.IdConvenio,
                    NumeroCarteira = dto.Convenio.Numero,
                    Validade = DateOnly.FromDateTime(dto.Convenio.Validade)
                };

                // 5. Salva o vínculo do convênio
                try
                {
                    await _pacienteConvenioRepository.AddAsync(pacienteConvenio);
                    await _pacienteConvenioRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"ERRO AO SALVAR VÍNCULO DO CONVÊNIO: {ex.GetBaseException().Message}", ex);
                }
            }
        }

        // BUSCAR PACIENTE PARA EDIÇÃO
        public async Task<PacienteCadastroDto?> ObterParaEdicaoAsync(int id)
        {
            var paciente = await _pacienteRepository.ObterComDetalhesAsync(id);

            if (paciente == null)
                return null;

            var dto = new PacienteCadastroDto
            {
                Nome = paciente.IdPessoaNavigation.Nome,
                Cpf = paciente.IdPessoaNavigation.Cpf,
                Telefone = paciente.IdPessoaNavigation.Telefone,
                DataNascimento = paciente.IdPessoaNavigation.Nascimento?
                    .ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                Sexo = paciente.IdPessoaNavigation.Sexo ?? "",
                Cep = paciente.IdPessoaNavigation.Cep ?? "",
                Rua = paciente.IdPessoaNavigation.Rua ?? string.Empty,
                Numero = paciente.IdPessoaNavigation.NumeroCasa ?? string.Empty,
                Bairro = paciente.IdPessoaNavigation.Bairro ?? string.Empty,
                Cidade = paciente.IdPessoaNavigation.Cidade ?? string.Empty,
                Estado = paciente.IdPessoaNavigation.Estado ?? string.Empty,

                Alergias = paciente.Alergias ?? "",
                TipoSanguineo = paciente.TipoSanguineo ?? "",
                HistoricoClinico = paciente.HistoricoClinico ?? "",
                NomeResponsavel = paciente.NomeResponsavel ?? ""
            };

            var pacienteConvenio = paciente.PacienteConvenios?.FirstOrDefault();

            if (pacienteConvenio != null)
            {
                Console.WriteLine("===== TESTE DO VÍNCULO =====");
                Console.WriteLine($"ID do vínculo: {pacienteConvenio.IdPacienteConvenio}");
                Console.WriteLine($"ID do paciente: {pacienteConvenio.IdPaciente}");
                Console.WriteLine($"ID do convênio: {pacienteConvenio.IdConvenio}");
                Console.WriteLine($"Nome do convênio: {pacienteConvenio.IdConvenioNavigation?.NomeConvenio}");
                Console.WriteLine("============================");
            }

            if (pacienteConvenio != null &&
                pacienteConvenio.IdConvenioNavigation != null)
            {
                dto.Convenio = new PacienteConvenioCadastroDto
                {
                    IdPaciente = pacienteConvenio.IdPaciente,
                    IdConvenio = pacienteConvenio.IdConvenio,
                    NomeConvenio = pacienteConvenio.IdConvenioNavigation.NomeConvenio,
                    Numero = pacienteConvenio.NumeroCarteira ?? "",
                    Validade = pacienteConvenio.Validade.ToDateTime(TimeOnly.MinValue),
                    Ativo = 1
                };
            }

            return dto;
        }

        // ATUALIZAR PACIENTE
        public async Task AtualizarAsync(int id, PacienteCadastroDto dto)
        {
            var paciente = await _pacienteRepository.ObterComDetalhesAsync(id);

            if (paciente == null)
                throw new Exception("Paciente não encontrado.");

            // Atualiza Pessoa
            paciente.IdPessoaNavigation.Nome = dto.Nome;
            paciente.IdPessoaNavigation.Cpf = dto.Cpf;
            paciente.IdPessoaNavigation.Telefone = dto.Telefone;
            paciente.IdPessoaNavigation.Email = dto.Email;
            paciente.IdPessoaNavigation.Nascimento = DateOnly.FromDateTime(dto.DataNascimento);
            paciente.IdPessoaNavigation.Sexo = dto.Sexo;
            paciente.IdPessoaNavigation.Cep = dto.Cep;
            paciente.IdPessoaNavigation.Rua = dto.Rua;
            paciente.IdPessoaNavigation.NumeroCasa = dto.Numero;
            paciente.IdPessoaNavigation.Bairro = dto.Bairro;
            paciente.IdPessoaNavigation.Cidade = dto.Cidade;
            paciente.IdPessoaNavigation.Estado = dto.Estado;

            // Atualiza Paciente
            paciente.Alergias = dto.Alergias;
            paciente.TipoSanguineo = dto.TipoSanguineo;
            paciente.HistoricoClinico = dto.HistoricoClinico;
            paciente.NomeResponsavel = dto.NomeResponsavel;

            _pacienteRepository.Update(paciente);

            // Atualiza Convênio
            var pcExistente = paciente.PacienteConvenios?.FirstOrDefault();

            if (dto.Convenio != null &&
                !string.IsNullOrWhiteSpace(dto.Convenio.NomeConvenio))
            {
                var convenios = await _convenioRepository.FindAsync(
                    c => c.NomeConvenio == dto.Convenio.NomeConvenio);

                var convenioDb = convenios.FirstOrDefault();

                if (convenioDb == null)
                {
                    throw new Exception(
                        $"Convênio '{dto.Convenio.NomeConvenio}' não encontrado.");
                }

                if (pcExistente != null)
                {
                    pcExistente.IdConvenio = convenioDb.IdConvenio;
                    pcExistente.NumeroCarteira = dto.Convenio.Numero;
                    pcExistente.Validade = DateOnly.FromDateTime(dto.Convenio.Validade);

                    _pacienteConvenioRepository.Update(pcExistente);
                }
                else
                {
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
            else
            {
                if (pcExistente != null)
                {
                    _pacienteConvenioRepository.Delete(pcExistente);
                }
            }

            await _pacienteRepository.SaveChangesAsync();

            // Limpa o cache após a atualização.
            InvalidarCachePacientes();
        }

        // LISTAR PACIENTES ATIVOS
        public async Task<IEnumerable<PacienteListagemDto>> ObterTodosAtivosAsync()
        {
            // Verifica se os dados já estão no cache.
            if (_cache.TryGetValue(
                CachePacientesAtivos,
                out List<PacienteListagemDto>? pacientesCacheados))
            {
                return pacientesCacheados!;
            }

            // Se não estiverem, consulta o banco.
            var pacientes = await _pacienteRepository.ObterTodosComPessoaAsync(ativo: 1);

            var resultado = pacientes.Select(p => new PacienteListagemDto
            {
                Id = p.IdPaciente,
                Nome = p.IdPessoaNavigation!.Nome,
                Cpf = p.IdPessoaNavigation.Cpf
            }).ToList();

            // Armazena por 5 minutos.
            _cache.Set(
                CachePacientesAtivos,
                resultado,
                TimeSpan.FromMinutes(5));

            return resultado;
        }

        // LISTAR PACIENTES INATIVOS
        public async Task<IEnumerable<PacienteListagemDto>> ObterTodosInativosAsync()
        {
            // Verifica se os dados já estão no cache.
            if (_cache.TryGetValue(
                CachePacientesInativos,
                out List<PacienteListagemDto>? pacientesCacheados))
            {
                return pacientesCacheados!;
            }

            // Se não estiverem, consulta o banco.
            var pacientes = await _pacienteRepository.ObterTodosComPessoaAsync(ativo: 0);

            var resultado = pacientes.Select(p => new PacienteListagemDto
            {
                Id = p.IdPaciente,
                Nome = p.IdPessoaNavigation!.Nome,
                Cpf = p.IdPessoaNavigation.Cpf
            }).ToList();

            // Armazena por 5 minutos.
            _cache.Set(
                CachePacientesInativos,
                resultado,
                TimeSpan.FromMinutes(5));

            return resultado;
        }

        // ARQUIVAR PACIENTE
        public async Task ArquivarAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);

            if (paciente != null)
            {
                paciente.Ativo = 0;
                _pacienteRepository.Update(paciente);
                await _pacienteRepository.SaveChangesAsync();

                InvalidarCachePacientes();
            }
        }

        // ATIVAR PACIENTE
        public async Task AtivarAsync(int id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);

            if (paciente != null)
            {
                paciente.Ativo = 1;
                _pacienteRepository.Update(paciente);
                await _pacienteRepository.SaveChangesAsync();

                InvalidarCachePacientes();
            }
        }

        // EXCLUIR PACIENTE
        public async Task ExcluirAsync(int id)
        {
            // 1. Buscar com os detalhes (Includes) para trazer os convênios.
            var paciente = await _pacienteRepository.ObterComDetalhesAsync(id);

            if (paciente == null)
                throw new Exception("Paciente não encontrado.");

            // 2. Proteger contra exclusão se houver histórico médico.
            if (paciente.Agendamentos != null && paciente.Agendamentos.Any())
            {
                throw new Exception("Não é possível excluir este paciente porque existem agendamentos vinculados. Utilize o arquivamento.");
            }

            if (paciente.Prontuarios != null && paciente.Prontuarios.Any())
            {
                throw new Exception("Não é possível excluir este paciente porque existem prontuários vinculados. Utilize o arquivamento.");
            }

            if (paciente.Triagems != null && paciente.Triagems.Any())
            {
                throw new Exception("Não é possível excluir este paciente porque existem triagens vinculadas. Utilize o arquivamento.");
            }

            try
            {
                // 3. Remover vínculos da tabela paciente_convenio primeiro.
                if (paciente.PacienteConvenios != null && paciente.PacienteConvenios.Any())
                {
                    foreach (var vinculo in paciente.PacienteConvenios.ToList())
                    {
                        _pacienteConvenioRepository.Delete(vinculo);
                    }

                    await _pacienteConvenioRepository.SaveChangesAsync();
                }

                // 4. Remover o registro principal.
                _pacienteRepository.Delete(paciente);

                // 5. Salvar a exclusão do paciente.
                await _pacienteRepository.SaveChangesAsync();

                // Limpa o cache após a exclusão bem-sucedida.
                InvalidarCachePacientes();
            }
            catch (Exception ex)
            {
                var erroReal = ex.GetBaseException().Message;
                throw new Exception($"Erro no banco ao excluir paciente: {erroReal}", ex);
            }
        }
    }
}
