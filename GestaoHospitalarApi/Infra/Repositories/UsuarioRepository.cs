using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
// Substitua pelo namespace correto do seu AppDbContext, se for diferente
using GestaoHospitalarApi.Infra.EF;


namespace GestaoHospitalarApi.Infra.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        // O construtor repassa o AppDbContext para a classe base (GenericRepository)
        public UsuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByLoginAsync(string login)
        {
            // _context é a instância do AppDbContext injetada na classe base.
            // Se o seu GenericRepository não expõe o _context como protected, 
            // você pode precisar alterar lá ou usar _context.Set<Usuario>()
            return await _context.Set<Usuario>()
                .Include(u => u.IdPessoaNavigation) // Traz os dados da Pessoa na mesma query
                .FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}