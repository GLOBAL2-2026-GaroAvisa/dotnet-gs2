using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Domain.Interfaces
{
    public interface ILocalRepository
    {
        Task<List<Local>> ListarAsync(int usuarioId);
        Task<Local?> BuscarPorIdAsync(int id, int usuarioId);
        Task<bool> ApelidoExisteAsync(string apelido, int usuarioId, int? ignorarId = null);
        Task<Local> CriarAsync(Local local);
        Task AtualizarAsync(Local local);
        Task ExcluirAsync(Local local);
        Task RemoverPrincipalAsync(int usuarioId, int? ignorarId = null);
        Task<List<Local>> ListarParaAlertasAsync();
    }
}
