using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GaroAvisa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public AuthController(IUsuarioService service)
        {
            _service = service;
        }

        /// <summary>Cria uma conta de usuário e já devolve o token de acesso.</summary>
        /// <response code="201">Conta criada.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="409">E-mail já cadastrado.</response>
        [HttpPost("cadastro")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginRespostaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Cadastrar([FromBody] CadastroUsuarioDto dto)
        {
            try
            {
                var resposta = await _service.CadastrarAsync(dto);
                return CreatedAtAction(nameof(Me), null, resposta);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>Faz login com e-mail e senha e devolve o token JWT.</summary>
        /// <response code="200">Login realizado.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="401">E-mail ou senha incorretos.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var resposta = await _service.LoginAsync(dto);
            return resposta is null
                ? Unauthorized(new { mensagem = "E-mail ou senha incorretos." })
                : Ok(resposta);
        }

        /// <summary>Mostra os dados do usuário que está logado.</summary>
        /// <response code="200">Dados do usuário.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UsuarioRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Me()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _service.BuscarPorIdAsync(usuarioId);
            return usuario is null ? Unauthorized() : Ok(usuario);
        }
    }
}