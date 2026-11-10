using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Domain.Interfaces
{
    public interface IIdempotenciaRepository
    {
        Task<RegistroIdempotencia?> BuscarAsync(string chave, int usuarioId, DateTime criadoDepoisDe);
        Task SalvarAsync(RegistroIdempotencia registro);
    }
}