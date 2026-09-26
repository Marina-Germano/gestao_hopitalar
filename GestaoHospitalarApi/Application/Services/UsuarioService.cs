using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Domain.Repositories;

namespace GestaoHospitalarApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllUsuariosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(u => new UsuarioDTO
            {
                IdUsuario = u.IdUsuario,
                Ativo = u.Ativo ?? 1, // Remova os ?? 1 se o seu DTO e Model baterem direitinho os tipos (int ou int?)
                Nome = u.Nome,
                Login = u.Login,
                Email = u.Email,
                Perfil = u.Perfil
            });
        }

        public async Task<UsuarioDTO> GetUsuarioByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return null;

            return new UsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                Ativo = usuario.Ativo ?? 1,
                Nome = usuario.Nome,
                Login = usuario.Login,
                Email = usuario.Email,
                Perfil = usuario.Perfil
            };
        }

        public async Task<UsuarioDTO> AddUsuarioAsync(UsuarioDTO usuarioDto)
        {
            var usuario = new Usuario
            {
                Ativo = usuarioDto.Ativo,
                Nome = usuarioDto.Nome,
                Login = usuarioDto.Login,
                Senha = usuarioDto.Senha, 
                Email = usuarioDto.Email,
                Perfil = usuarioDto.Perfil
            };

            // Prepara a inserção na memória
            await _usuarioRepository.AddAsync(usuario);
            
            // EXECUTA a ação no banco de dados. É AQUI que o ID será gerado!
            await _usuarioRepository.SaveChangesAsync(); 

            // Agora o usuário já tem o ID preenchido pelo Entity Framework
            usuarioDto.IdUsuario = usuario.IdUsuario;
            
            return usuarioDto;
        }

        public async Task UpdateUsuarioAsync(UsuarioDTO usuarioDto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioDto.IdUsuario);
            if (usuario == null) throw new Exception("Usuário não encontrado.");

            usuario.Ativo = usuarioDto.Ativo;
            usuario.Nome = usuarioDto.Nome;
            usuario.Email = usuarioDto.Email;
            usuario.Perfil = usuarioDto.Perfil;
            
            if (!string.IsNullOrEmpty(usuarioDto.Senha))
                usuario.Senha = usuarioDto.Senha;

            // Chama o Update (sem await, pois é void no seu GenericRepository)
            _usuarioRepository.Update(usuario);
            
            // Salva as alterações no banco
            await _usuarioRepository.SaveChangesAsync();
        }

        public async Task DeleteUsuarioAsync(int id)
        {
            // Primeiro, busca a entidade inteira pelo ID
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            
            if (usuario != null)
            {
                // Deleta passando o objeto inteiro (sem await, pois é void)
                _usuarioRepository.Delete(usuario);
                
                // Salva a alteração no banco
                await _usuarioRepository.SaveChangesAsync();
            }
        }
    }
}