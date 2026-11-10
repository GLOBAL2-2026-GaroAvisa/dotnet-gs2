using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GaroAvisa.Infrastructure.Repositories
{
    public class AlertaRepository : IAlertaRepository
    {
        private readonly AppDbContext _context;

        public AlertaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Alerta>> ListarPorLocaisAsync(List<int> localIds, bool somenteNaoLidos)
        {
            return await _context.Alertas
                .Where(a => localIds.Contains(a.LocalId) && (!somenteNaoLidos || !a.Lido))
                .OrderByDescending(a => a.DataPrevisao)
                .ThenByDescending(a => a.CriadoEm)
                .ToListAsync();
        }

        public async Task<List<Alerta>> ListarPorLocalEDataAsync(int localId, DateTime data)
        {
            return await _context.Alertas
                .Where(a => a.LocalId == localId && a.DataPrevisao == data)
                .OrderByDescending(a => a.CriadoEm)
                .ToListAsync();
        }

        public async Task<Alerta?> BuscarPorIdAsync(int id)
        {
            return await _context.Alertas.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> ExisteAsync(int localId, DateTime data, string tipo)
        {
            return await _context.Alertas.AnyAsync(a =>
                a.LocalId == localId && a.DataPrevisao == data && a.Tipo == tipo);
        }

        public async Task<Alerta> CriarAsync(Alerta alerta)
        {
            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();
            return alerta;
        }

        public async Task AtualizarAsync(Alerta alerta)
        {
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(Alerta alerta)
        {
            _context.Alertas.Remove(alerta);
            await _context.SaveChangesAsync();
        }
    }
}