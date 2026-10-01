using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class EspecialidadeRepository : GenericRepository<Especialidade>, IEspecialidadeRepository
    {
        public EspecialidadeRepository(AppDbContext context) : base(context)
        {
            //quando eu precisar de algum método específico do contexto, posso usar o _context aqui
            //metodos alem do CRUD padrão
        }
    }
}