using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface IOpenMeteoClient
    {
        Task<List<LugarDto>> BuscarLugaresAsync(string nome);
        Task<List<PrevisaoDiaDto>> ObterPrevisaoAsync(double latitude, double longitude, string fuso, int dias);
    }
}