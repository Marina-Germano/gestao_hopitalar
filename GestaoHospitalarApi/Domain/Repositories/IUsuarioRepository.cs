using GestaoHospitalarApi.Models;

namespace GestaoHospitalarApi.Domain.Repositories
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        // Aqui entram os métodos específicos que não são apenas o CRUD padrão
        Task<Usuario?> GetByLoginAsync(string login);
        //Task<Usuario> GetByEmailAsync(string email);
    }
}