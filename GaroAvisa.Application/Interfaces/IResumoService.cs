using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface IResumoService
    {
        Task<ResumoDiaDto> ObterResumoAsync(int usuarioId);
    }
}