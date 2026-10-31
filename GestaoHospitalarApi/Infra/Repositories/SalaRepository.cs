using GestaoHospitalarApi.Domain.Repositories;
using GestaoHospitalarApi.Models;
using Microsoft.EntityFrameworkCore;
using GestaoHospitalarApi.Infra.EF;

namespace GestaoHospitalarApi.Infra.Repositories
{
    public class SalaRepository : GenericRepository<Sala>, ISalaRepository
    {
        public SalaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Sala?> ObterComAlaAsync(int id)
        {
            return await _context.Set<Sala>()
                .Include(s => s.IdAlaNavigation)
                .FirstOrDefaultAsync(s => s.IdSala == id);
        }

        public async Task<IEnumerable<Sala>> ObterTodasComAlaAsync()
{
    var salas = await _context.Set<Sala>().ToListAsync();

    Console.WriteLine($"TOTAL DE SALAS ENCONTRADAS: {salas.Count}");

    foreach (var sala in salas)
    {
        Console.WriteLine(
            $"ID: {sala.IdSala} | Nome: {sala.Nome} | Ala: {sala.IdAla}"
        );
    }

    return salas;
}

        public async Task<IEnumerable<Sala>> ObterSalasLivresAsync()
        {
            return await _context.Set<Sala>()
                .Include(s => s.IdAlaNavigation)
                .Where(s => s.Status == "LIVRE")
                .OrderBy(s => s.IdAlaNavigation.NomeAla)
                .ThenBy(s => s.Nome)
                .ToListAsync();
        }
    }
}