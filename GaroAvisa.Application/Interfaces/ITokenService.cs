using GaroAvisa.Domain.Entities;

namespace GaroAvisa.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Usuario usuario);
    }
}
