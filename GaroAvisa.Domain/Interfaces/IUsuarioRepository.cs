using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> BuscarPorIdAsync(int id);
        Task<Usuario?> BuscarPorEmailAsync(string email);
        Task<bool> EmailExisteAsync(string email);
        Task<Usuario> CriarAsync(Usuario usuario);
    }
}
