using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface IPreferenciaService
    {
        Task<PreferenciaRespostaDto> ObterAsync(int usuarioId);
        Task<PreferenciaRespostaDto> SalvarAsync(SalvarPreferenciaDto dto, int usuarioId);
        Task RestaurarPadraoAsync(int usuarioId);
    }
}