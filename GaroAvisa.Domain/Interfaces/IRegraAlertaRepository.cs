using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Domain.Interfaces
{
    public interface IRegraAlertaRepository
    {
        Task<List<RegraAlerta>> ListarAsync(bool somenteAtivas);
        Task<RegraAlerta?> BuscarPorIdAsync(int id);
        Task<bool> ExisteAsync(string tipo, string gravidade, int? ignorarId = null);
        Task<RegraAlerta> CriarAsync(RegraAlerta regra);
        Task AtualizarAsync(RegraAlerta regra);
        Task ExcluirAsync(RegraAlerta regra);
    }
}