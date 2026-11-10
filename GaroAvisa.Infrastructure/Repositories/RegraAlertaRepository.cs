using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Repositories
{
    public class RegraAlertaRepository : IRegraAlertaRepository
    {
        private readonly AppDbContext _context;

        public RegraAlertaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RegraAlerta>> ListarAsync(bool somenteAtivas)
        {
            return await _context.RegrasAlerta
                .Where(r => !somenteAtivas || r.Ativa)
                .OrderBy(r => r.Tipo)
                .ThenBy(r => r.Limite)
                .ToListAsync();
        }

        public async Task<RegraAlerta?> BuscarPorIdAsync(int id)
        {
            return await _context.RegrasAlerta.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> ExisteAsync(string tipo, string gravidade, int? ignorarId = null)
        {
            return await _context.RegrasAlerta.AnyAsync(r =>
                r.Tipo == tipo &&
                r.Gravidade == gravidade &&
                (ignorarId == null || r.Id != ignorarId));
        }

        public async Task<RegraAlerta> CriarAsync(RegraAlerta regra)
        {
            _context.RegrasAlerta.Add(regra);
            await _context.SaveChangesAsync();
            return regra;
        }

        public async Task AtualizarAsync(RegraAlerta regra)
        {
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(RegraAlerta regra)
        {
            _context.RegrasAlerta.Remove(regra);
            await _context.SaveChangesAsync();
        }
    }
}