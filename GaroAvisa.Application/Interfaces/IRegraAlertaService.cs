using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface IRegraAlertaService
    {
        Task<List<RegraAlertaRespostaDto>> ListarAsync(bool somenteAtivas);
        Task<RegraAlertaRespostaDto?> BuscarPorIdAsync(int id);
        Task<RegraAlertaRespostaDto> CriarAsync(SalvarRegraAlertaDto dto);
        Task<RegraAlertaRespostaDto?> AtualizarAsync(int id, SalvarRegraAlertaDto dto);
        Task<bool> ExcluirAsync(int id);
    }
}