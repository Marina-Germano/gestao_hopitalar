using GestaoHospitalarApi.Application.Authentication;
using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;

        private readonly JwtService _jwtService;

        public UsuarioService(
            IGenericRepository<Usuario> usuarioRepository,
            JwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
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

        public async Task<string?> LoginAsync(UsuarioLoginDTO dto)
        {
            var usuarios = await _usuarioRepository.FindAsync(
                u => u.Login == dto.Login
            );

            var usuario = usuarios.FirstOrDefault();

            if (usuario == null)
                return null;

            if (usuario.Senha != dto.Senha)
                return null;

            if (usuario.Ativo != 1)
                return null;

            var token = _jwtService.GenerateToken(
                usuario.IdUsuario,
                usuario.Login,
                usuario.Perfil
            );

            return token;
        }
    }
}