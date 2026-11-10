using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Domain.Interfaces
{
    public interface IPrevisaoSalvaRepository
    {
        Task<PrevisaoSalva?> BuscarAsync(int localId, DateTime data);
        Task<PrevisaoSalva> CriarAsync(PrevisaoSalva previsao);
        Task AtualizarAsync(PrevisaoSalva previsao);
    }
}