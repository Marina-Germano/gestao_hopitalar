using GestaoHospitalarApi.Application.Authentication;
using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.AspNetCore.Identity;

namespace GestaoHospitalarApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepository;
        private readonly JwtService _jwtService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(
            IGenericRepository<Usuario> usuarioRepository,
            JwtService jwtService,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        public async Task<UsuarioDTO> AddUsuarioAsync(UsuarioCadastroDto dto)
        {
            var perfilValidado = dto.Perfil.Trim().ToUpper();

            var usuario = new Usuario
            {
                Login = dto.Login,
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
                    NumeroCasa = dto.NumeroCasa?.ToString()?? string.Empty,
                    Bairro = dto.Bairro,
                    Cidade = dto.Cidade,
                    Estado = dto.Estado,
                    Cep = dto.Cep
                }
            };

            usuario.Senha = _passwordHasher.HashPassword(
                usuario,
                dto.Senha
            );

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

        if (usuario == null || usuario.Ativo != 1)
            return null;

        PasswordVerificationResult resultado;

        try
        {
            resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Senha,
                dto.Senha
            );
        }
        catch (FormatException)
        {
            // Compatibilidade com senhas antigas armazenadas em texto puro.
            // Se a senha estiver correta, ela será convertida para hash.
            if (!string.Equals(
                usuario.Senha,
                dto.Senha,
                StringComparison.Ordinal))
            {
                return null;
            }

            resultado = PasswordVerificationResult.SuccessRehashNeeded;
        }

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        // Atualiza senhas antigas para o formato de hash.
        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.Senha = _passwordHasher.HashPassword(
                usuario,
                dto.Senha
            );

            _usuarioRepository.Update(usuario);

            await _usuarioRepository.SaveChangesAsync();
        }

        var token = _jwtService.GenerateToken(
            usuario.IdUsuario,
            usuario.Login,
            usuario.Perfil
        );

        return token;
    }

        }
}