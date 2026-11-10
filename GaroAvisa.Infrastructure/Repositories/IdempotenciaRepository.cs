using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly AppDbContext _context;

        public IdempotenciaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RegistroIdempotencia?> BuscarAsync(string chave, int usuarioId, DateTime criadoDepoisDe)
        {
            return await _context.RegistrosIdempotencia
                .FirstOrDefaultAsync(r => r.Chave == chave && r.UsuarioId == usuarioId && r.CriadoEm > criadoDepoisDe);
        }

        public async Task SalvarAsync(RegistroIdempotencia registro)
        {
            _context.RegistrosIdempotencia.Add(registro);
            await _context.SaveChangesAsync();
        }
    }
}