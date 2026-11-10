using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface IAlertaService
    {
        Task<List<AlertaRespostaDto>> ListarAsync(int usuarioId, bool somenteNaoLidos);
        Task<AlertaRespostaDto?> BuscarPorIdAsync(int id, int usuarioId);
        Task<AlertaRespostaDto?> MarcarComoLidoAsync(int id, int usuarioId);
        Task<bool> ExcluirAsync(int id, int usuarioId);
        Task<VerificacaoResultadoDto> VerificarAsync();
    }
}