using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Repositories
{
    public class PrevisaoSalvaRepository : IPrevisaoSalvaRepository
    {
        private readonly AppDbContext _context;

        public PrevisaoSalvaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PrevisaoSalva?> BuscarAsync(int localId, DateTime data)
        {
            return await _context.PrevisoesSalvas
                .FirstOrDefaultAsync(p => p.LocalId == localId && p.DataPrevisao == data);
        }

        public async Task<PrevisaoSalva> CriarAsync(PrevisaoSalva previsao)
        {
            _context.PrevisoesSalvas.Add(previsao);
            await _context.SaveChangesAsync();
            return previsao;
        }

        public async Task AtualizarAsync(PrevisaoSalva previsao)
        {
            await _context.SaveChangesAsync();
        }
    }
}