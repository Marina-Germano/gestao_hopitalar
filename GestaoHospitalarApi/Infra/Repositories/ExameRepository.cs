using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Infra.EF;
namespace GestaoHospitalarApi.Infra.Repositories
{
    public class ExameRepository : GenericRepository<Exame>, IExameRepository
    {
        public ExameRepository(AppDbContext context) : base(context) { }
    }
}