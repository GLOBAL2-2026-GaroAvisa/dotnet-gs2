using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface ILocalService
    {
        Task<List<LocalRespostaDto>> ListarAsync(int usuarioId);
        Task<LocalRespostaDto?> BuscarPorIdAsync(int id, int usuarioId);
        Task<LocalRespostaDto> CriarAsync(CriarLocalDto dto, int usuarioId);
        Task<LocalRespostaDto?> AtualizarAsync(int id, AtualizarLocalDto dto, int usuarioId);
        Task<bool> ExcluirAsync(int id, int usuarioId);
    }
}
