using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Domain.Interfaces
{
    public interface IPreferenciaRepository
    {
        Task<Preferencia?> BuscarPorUsuarioAsync(int usuarioId);
        Task<Preferencia> CriarAsync(Preferencia preferencia);
        Task AtualizarAsync(Preferencia preferencia);
        Task ExcluirAsync(Preferencia preferencia);
    }
}