using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using GestaoHospitalarApi.Infra.EF; // Ajuste se o namespace do seu DbContext for diferente

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class ConvenioRepository : GenericRepository<Convenio>, IConvenioRepository
    {
        public ConvenioRepository(AppDbContext context) : base(context)
        {
            //quando eu precisar de algum método específico do contexto, posso usar o _context aqui
            //metodos alem do CRUD padrão
        }
    }
}