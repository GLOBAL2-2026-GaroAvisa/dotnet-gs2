using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Repositories
{
    public class PreferenciaRepository : IPreferenciaRepository
    {
        private readonly AppDbContext _context;

        public PreferenciaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Preferencia?> BuscarPorUsuarioAsync(int usuarioId)
        {
            return await _context.Preferencias.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }

        public async Task<Preferencia> CriarAsync(Preferencia preferencia)
        {
            _context.Preferencias.Add(preferencia);
            await _context.SaveChangesAsync();
            return preferencia;
        }

        public async Task AtualizarAsync(Preferencia preferencia)
        {
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(Preferencia preferencia)
        {
            _context.Preferencias.Remove(preferencia);
            await _context.SaveChangesAsync();
        }
    }
}