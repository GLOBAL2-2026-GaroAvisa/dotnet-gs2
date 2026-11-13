using GaroAvisa.Application.DTOs;

namespace GaroAvisa.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<LoginRespostaDto> CadastrarAsync(CadastroUsuarioDto dto);
        Task<LoginRespostaDto?> LoginAsync(LoginDto dto);
        Task<UsuarioRespostaDto?> BuscarPorIdAsync(int id);
        Task<bool> ExcluirContaAsync(int id);
    }
}
