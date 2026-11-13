using GaroAvisa.Infrastructure.Data;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> BuscarPorIdAsync(int id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<Usuario> CriarAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task ExcluirAsync(Usuario usuario)
        {
            var registros = await _context.RegistrosIdempotencia
                .Where(r => r.UsuarioId == usuario.Id)
                .ToListAsync();

            _context.RegistrosIdempotencia.RemoveRange(registros);
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
