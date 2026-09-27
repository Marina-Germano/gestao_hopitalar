using GestaoHospitalarApi.Application.DTOs;
using GestaoHospitalarApi.Models; // Ajuste para a pasta dos seus models
using GestaoHospitalarApi.Infra.EF; // Ajuste para a pasta do AppDbContext

namespace GestaoHospitalarApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioDTO> AddUsuarioAsync(UsuarioCreateDTO dto)
        {
            // 1. Validação básica (exemplo)
            var perfilValidado = dto.Perfil.Trim().ToUpper();

            // 2. Mapeamento para a Entidade
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                Nascimento = dto.Nascimento,
                Sexo = dto.Sexo?.ToUpper(),
                Telefone = dto.Telefone,
                Email = dto.Email,
                Rua = dto.Rua,
                NumeroCasa = dto.NumeroCasa,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                Cep = dto.Cep,
                Login = dto.Login,
                Senha = dto.Senha,
                Perfil = perfilValidado,
                Ativo = 1
            };

            // 3. Salva no banco de dados
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            // 4. Mapeia a Entidade para o DTO de Resposta (escondendo a senha)
            return new UsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nome = usuario.Nome,
                Login = usuario.Login,
                Perfil = usuario.Perfil
            };
        }
    }
}