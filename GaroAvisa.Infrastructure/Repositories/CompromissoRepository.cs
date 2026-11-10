using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Repositories
{
    public class CompromissoRepository : ICompromissoRepository
    {
        private readonly AppDbContext _context;

        public CompromissoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Compromisso>> ListarPorLocaisAsync(List<int> localIds)
        {
            return await _context.Compromissos
                .Where(c => localIds.Contains(c.LocalId))
                .OrderBy(c => c.DataHora)
                .ToListAsync();
        }

        public async Task<Compromisso?> BuscarPorIdAsync(int id)
        {
            return await _context.Compromissos.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Compromisso> CriarAsync(Compromisso compromisso)
        {
            _context.Compromissos.Add(compromisso);
            await _context.SaveChangesAsync();
            return compromisso;
        }

        public async Task AtualizarAsync(Compromisso compromisso)
        {
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(Compromisso compromisso)
        {
            _context.Compromissos.Remove(compromisso);
            await _context.SaveChangesAsync();
        }
    }
}