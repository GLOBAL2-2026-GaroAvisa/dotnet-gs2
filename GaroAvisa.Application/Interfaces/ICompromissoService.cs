using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface ICompromissoService
    {
        Task<List<CompromissoRespostaDto>> ListarAsync(int usuarioId, int? localId);
        Task<CompromissoRespostaDto?> BuscarPorIdAsync(int id, int usuarioId);
        Task<CompromissoRespostaDto?> CriarAsync(SalvarCompromissoDto dto, int usuarioId);
        Task<CompromissoRespostaDto?> AtualizarAsync(int id, SalvarCompromissoDto dto, int usuarioId);
        Task<bool> ExcluirAsync(int id, int usuarioId);
        Task<CompromissoPrevisaoDto?> ObterPrevisaoAsync(int id, int usuarioId);
    }
}