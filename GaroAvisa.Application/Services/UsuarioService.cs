using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GaroAvisa.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public UsuarioService(IUsuarioRepository repository, ITokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public async Task<LoginRespostaDto> CadastrarAsync(CadastroUsuarioDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            if (await _repository.EmailExisteAsync(email))
            {
                throw new InvalidOperationException("Já existe uma conta com esse e-mail. Faça login ou use outro e-mail.");
            }

            var usuario = new Usuario
            {
                Nome = dto.Nome.Trim(),
                Email = email,
                Tipo = "USUARIO"
            };

            usuario.SenhaHash = _passwordHasher.HashPassword(usuario, dto.Senha);

            var criado = await _repository.CriarAsync(usuario);
            return GerarResposta(criado);
        }

        public async Task<LoginRespostaDto?> LoginAsync(LoginDto dto)
        {
            var usuario = await _repository.BuscarPorEmailAsync(dto.Email.Trim().ToLower());
            if (usuario is null)
            {
                return null;
            }

            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, dto.Senha);
            if (resultado == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return GerarResposta(usuario);
        }

        public async Task<UsuarioRespostaDto?> BuscarPorIdAsync(int id)
        {
            var usuario = await _repository.BuscarPorIdAsync(id);
            return usuario is null ? null : ParaResposta(usuario);
        }

        public async Task<bool> ExcluirContaAsync(int id)
        {
            var usuario = await _repository.BuscarPorIdAsync(id);
            if (usuario is null)
            {
                return false;
            }

            if (usuario.Tipo == "ADMIN")
            {
                throw new InvalidOperationException("Contas de administrador não podem ser excluídas pela API, para o sistema não ficar sem administrador.");
            }

            await _repository.ExcluirAsync(usuario);
            return true;
        }

        private LoginRespostaDto GerarResposta(Usuario usuario)
        {
            return new LoginRespostaDto
            {
                Token = _tokenService.GenerateToken(usuario),
                Usuario = ParaResposta(usuario)
            };
        }

        private static UsuarioRespostaDto ParaResposta(Usuario usuario)
        {
            return new UsuarioRespostaDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Tipo = usuario.Tipo
            };
        }
    }
}
