using GestaoHospitalarApi.Application.DTOs.Medico;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Identity;

namespace GestaoHospitalarApi.Application.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IMedicoRepository _medicoRepository;
        private readonly IGenericRepository<Especialidade> _especialidadeRepository;
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public MedicoService(
            IMedicoRepository medicoRepository,
            IGenericRepository<Especialidade> especialidadeRepository,
            IGenericRepository<Usuario> usuarioRepository,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _medicoRepository = medicoRepository;
            _especialidadeRepository = especialidadeRepository;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task CadastrarAsync(MedicoCadastroDto dto)
        {
            // 1. Busca ou Cria a Especialidade
            var especialidades = await _especialidadeRepository.FindAsync(e => e.DescricaoEspecialidade.ToLower() == dto.DescricaoEspecialidade.ToLower());
            var especialidade = especialidades.FirstOrDefault();

            if (especialidade == null)
            {
                especialidade = new Especialidade { DescricaoEspecialidade = dto.DescricaoEspecialidade };
                await _especialidadeRepository.AddAsync(especialidade);
                await _especialidadeRepository.SaveChangesAsync(); 
            }

            // 2. Cria a Pessoa vinculada ao Usuário
            var usuario = new Usuario
            {
                Login = dto.Crm,
                Perfil = dto.Perfil,
                Ativo = 1,
                IdPessoaNavigation = new Pessoa
                {
                    Nome = dto.Nome ?? string.Empty,
                    Cpf = dto.Cpf ?? string.Empty,
                    Telefone = dto.Telefone,
                    Email = dto.Email,
                    Nascimento = DateOnly.FromDateTime(dto.DataNascimento),
                    Sexo = dto.Sexo?.ToUpper(),
                    Cep = dto.Cep ?? string.Empty,
                    Rua = dto.Rua ?? string.Empty,
                    NumeroCasa = dto.Numero ?? string.Empty,
                    Bairro = dto.Bairro ?? string.Empty,
                    Cidade = dto.Cidade ?? string.Empty,
                    Estado = dto.Estado ?? string.Empty
                }
            };
            usuario.Senha = _passwordHasher.HashPassword(usuario, dto.Senha);

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveChangesAsync(); // Gera IdPessoa e IdUsuario

            // 3. Cria o Médico vinculado APENAS ao Usuário e Especialidade
            var medico = new Medico
            {
                Crm = dto.Crm,
                Honorario = dto.Honorario,
                IdEspecialidade = especialidade.IdEspecialidade,
                IdUsuario = usuario.IdUsuario // Vínculo direto com o usuário recém criado!
            };

            await _medicoRepository.AddAsync(medico);
            await _medicoRepository.SaveChangesAsync();
        }

        public async Task<MedicoCadastroDto?> ObterParaEdicaoAsync(int id)
        {
            var medico = await _medicoRepository.ObterComDetalhesAsync(id);
            if (medico == null || medico.IdUsuarioNavigation == null || medico.IdUsuarioNavigation.IdPessoaNavigation == null) 
                return null;

            var usuario = medico.IdUsuarioNavigation;
            var pessoa = usuario.IdPessoaNavigation;

            return new MedicoCadastroDto
            {
                // Dados da Pessoa (através do usuário)
                Nome = pessoa.Nome,
                Cpf = pessoa.Cpf,
                Telefone = pessoa.Telefone,
                Email = pessoa.Email,
                DataNascimento = pessoa.Nascimento?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
                Sexo = pessoa.Sexo ?? "",
                Cep = pessoa.Cep ?? "",
                Rua = pessoa.Rua ?? "",
                Numero = pessoa.NumeroCasa ?? "",
                Bairro = pessoa.Bairro ?? "",
                Cidade = pessoa.Cidade ?? "",
                Estado = pessoa.Estado ?? "",
                
                // Dados do Médico
                Crm = medico.Crm ?? "",
                Honorario = medico.Honorario,
                DescricaoEspecialidade = medico.IdEspecialidadeNavigation?.DescricaoEspecialidade ?? "",
                
                // Dados de Acesso (através do usuário)
                Perfil = usuario.Perfil ?? "",
                Senha = "" 
            };
        }

        public async Task AtualizarAsync(int id, MedicoCadastroDto dto)
        {
            var medico = await _medicoRepository.ObterComDetalhesAsync(id);
            if (medico == null || medico.IdUsuarioNavigation == null || medico.IdUsuarioNavigation.IdPessoaNavigation == null) 
                throw new Exception("Médico ou dados de usuário não encontrados.");

            // 1. Trata Especialidade
            var especialidades = await _especialidadeRepository.FindAsync(e => e.DescricaoEspecialidade.ToLower() == dto.DescricaoEspecialidade.ToLower());
            var especialidade = especialidades.FirstOrDefault();
            if (especialidade == null)
            {
                especialidade = new Especialidade { DescricaoEspecialidade = dto.DescricaoEspecialidade };
                await _especialidadeRepository.AddAsync(especialidade);
                await _especialidadeRepository.SaveChangesAsync();
            }

            var usuario = medico.IdUsuarioNavigation;
            var pessoa = usuario.IdPessoaNavigation;

            // 2. Atualiza Dados Pessoais
            pessoa.Nome = dto.Nome;
            pessoa.Cpf = dto.Cpf;
            pessoa.Telefone = dto.Telefone;
            pessoa.Email = dto.Email;
            pessoa.Nascimento = DateOnly.FromDateTime(dto.DataNascimento);
            pessoa.Sexo = dto.Sexo;
            pessoa.Cep = dto.Cep;
            pessoa.Rua = dto.Rua;
            pessoa.NumeroCasa = dto.Numero;
            pessoa.Bairro = dto.Bairro;
            pessoa.Cidade = dto.Cidade;
            pessoa.Estado = dto.Estado;

            // 3. Atualiza Dados de Acesso (Usuário)
            usuario.Login = dto.Crm;
            usuario.Perfil = dto.Perfil;
            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                usuario.Senha = _passwordHasher.HashPassword(usuario, dto.Senha);
            }

            // 4. Atualiza Dados do Médico
            medico.Crm = dto.Crm;
            medico.Honorario = dto.Honorario;
            medico.IdEspecialidade = especialidade.IdEspecialidade;

            _medicoRepository.Update(medico); // O EF Core é inteligente e atualizará o grafo inteiro (Médico, Usuário e Pessoa)
            await _medicoRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<MedicoListagemDto>> ObterTodosAtivosAsync()
        {
            var medicos = await _medicoRepository.ObterTodosComPessoaEEspecialidadeAsync(1);
            return medicos.Select(m => new MedicoListagemDto
            {
                Id = m.IdMedico,
                Nome = m.IdUsuarioNavigation!.IdPessoaNavigation!.Nome, // Caminho correto
                Crm = m.Crm ?? "",
                Especialidade = m.IdEspecialidadeNavigation?.DescricaoEspecialidade ?? ""
            });
        }

        public async Task<IEnumerable<MedicoListagemDto>> ObterTodosInativosAsync()
        {
            var medicos = await _medicoRepository.ObterTodosComPessoaEEspecialidadeAsync(0);
            return medicos.Select(m => new MedicoListagemDto
            {
                Id = m.IdMedico,
                Nome = m.IdUsuarioNavigation!.IdPessoaNavigation!.Nome, // Caminho correto
                Crm = m.Crm ?? "",
                Especialidade = m.IdEspecialidadeNavigation?.DescricaoEspecialidade ?? ""
            });
        }

        public async Task ArquivarAsync(int id)
        {
            var medico = await _medicoRepository.ObterComDetalhesAsync(id); // Usa o método com Includes
            if (medico != null && medico.IdUsuarioNavigation != null)
            {
                medico.IdUsuarioNavigation.Ativo = 0; // O arquivamento acontece no Usuário!
                _medicoRepository.Update(medico);
                await _medicoRepository.SaveChangesAsync();
            }
        }

        public async Task AtivarAsync(int id)
        {
            var medico = await _medicoRepository.ObterComDetalhesAsync(id);
            if (medico != null && medico.IdUsuarioNavigation != null)
            {
                medico.IdUsuarioNavigation.Ativo = 1;
                _medicoRepository.Update(medico);
                await _medicoRepository.SaveChangesAsync();
            }
        }

        public async Task ExcluirAsync(int id)
        {
            // O ideal é excluir o Médico e o Usuário para não deixar sujeira
            var medico = await _medicoRepository.ObterComDetalhesAsync(id);
            if (medico != null)
            {
                var usuario = medico.IdUsuarioNavigation;
                
                _medicoRepository.Delete(medico);
                
                // Se o banco não tiver Delete em Cascata, apagamos o usuário manualmente
                if (usuario != null) 
                    _usuarioRepository.Delete(usuario);

                await _medicoRepository.SaveChangesAsync();
            }
        }
    }
}