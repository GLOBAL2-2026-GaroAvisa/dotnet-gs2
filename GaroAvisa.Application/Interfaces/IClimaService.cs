using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface IClimaService
    {
        Task<List<LugarDto>> BuscarLugaresAsync(string nome);
        Task<PrevisaoLocalDto?> ObterPrevisaoDoLocalAsync(int localId, int usuarioId, int dias);
    }
}