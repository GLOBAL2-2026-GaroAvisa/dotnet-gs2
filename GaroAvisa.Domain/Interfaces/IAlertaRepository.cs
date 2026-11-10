using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Domain.Interfaces
{
    public interface IAlertaRepository
    {
        Task<List<Alerta>> ListarPorLocaisAsync(List<int> localIds, bool somenteNaoLidos);
        Task<List<Alerta>> ListarPorLocalEDataAsync(int localId, DateTime data);
        Task<Alerta?> BuscarPorIdAsync(int id);
        Task<bool> ExisteAsync(int localId, DateTime data, string tipo);
        Task<Alerta> CriarAsync(Alerta alerta);
        Task AtualizarAsync(Alerta alerta);
        Task ExcluirAsync(Alerta alerta);
    }
}