using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;

        public UsuarioService(IGenericRepository<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioDTO> AddUsuarioAsync(UsuarioCadastroDto dto)
        {
            var perfilValidado = dto.Perfil.Trim().ToUpper();

            // Instancia o Usuario com a Pessoa vinculada em IdPessoaNavigation
            var usuario = new Usuario
            {
                Login = dto.Login,
                Senha = dto.Senha,
                Perfil = perfilValidado,
                Ativo = 1,
                IdPessoaNavigation = new Pessoa
                {
                    Nome = dto.Nome,
                    Cpf = dto.Cpf,
                    Nascimento = dto.Nascimento.HasValue
                        ? DateOnly.FromDateTime(dto.Nascimento.Value)
                        : null,
                    Sexo = dto.Sexo?.ToUpper(),
                    Telefone = dto.Telefone,
                    Email = dto.Email,
                    Rua = dto.Rua,
                    NumeroCasa = dto.NumeroCasa,
                    Bairro = dto.Bairro,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    Cep = dto.Cep
                }
            };

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return new UsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.IdPessoaNavigation.Nome,
                Login = usuario.Login,
                Perfil = usuario.Perfil
            };
        }
    }
}