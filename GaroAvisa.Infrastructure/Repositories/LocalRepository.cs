using GaroAvisa.Infrastructure.Data;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Repositories
{
    public class LocalRepository : ILocalRepository
    {
        private readonly AppDbContext _context;

        public LocalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Local>> ListarAsync(int usuarioId)
        {
            return await _context.Locais
                .Where(l => l.UsuarioId == usuarioId)
                .OrderByDescending(l => l.Principal)
                .ThenBy(l => l.Apelido)
                .ToListAsync();
        }

        public async Task<Local?> BuscarPorIdAsync(int id, int usuarioId)
        {
            return await _context.Locais
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == usuarioId);
        }

        public async Task<bool> ApelidoExisteAsync(string apelido, int usuarioId, int? ignorarId = null)
        {
            return await _context.Locais.AnyAsync(l =>
                l.UsuarioId == usuarioId &&
                l.Apelido == apelido &&
                (ignorarId == null || l.Id != ignorarId));
        }

        public async Task<Local> CriarAsync(Local local)
        {
            _context.Locais.Add(local);
            await _context.SaveChangesAsync();
            return local;
        }

        public async Task AtualizarAsync(Local local)
        {
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(Local local)
        {
            _context.Locais.Remove(local);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverPrincipalAsync(int usuarioId, int? ignorarId = null)
        {
            var principais = await _context.Locais
                .Where(l => l.UsuarioId == usuarioId && l.Principal && (ignorarId == null || l.Id != ignorarId))
                .ToListAsync();

            foreach (var local in principais)
            {
                local.Principal = false;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<List<Local>> ListarParaAlertasAsync()
        {
            return await _context.Locais
                .Where(l => l.ReceberAlertas)
                .ToListAsync();
        }
    }
}
