using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GaroAvisa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PreferenciasController : ControllerBase
    {
        private readonly IPreferenciaService _service;

        public PreferenciasController(IPreferenciaService service)
        {
            _service = service;
        }

        /// <summary>Mostra quais notificações o usuário logado quer receber e o horário do resumo diário.</summary>
        /// <remarks>Se o usuário ainda não salvou nada, devolve o padrão: tudo ligado e resumo às 07:00.</remarks>
        /// <response code="200">Preferências retornadas.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PreferenciaRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Obter()
        {
            return Ok(await _service.ObterAsync(ObterUsuarioId()));
        }

        /// <summary>Salva as preferências do usuário logado.</summary>
        /// <remarks>Na primeira vez cria o registro; nas próximas, atualiza.</remarks>
        /// <response code="200">Preferências salvas.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpPut]
        [ProducesResponseType(typeof(PreferenciaRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Salvar([FromBody] SalvarPreferenciaDto dto)
        {
            return Ok(await _service.SalvarAsync(dto, ObterUsuarioId()));
        }

        /// <summary>Apaga as preferências do usuário logado e volta ao padrão.</summary>
        /// <response code="200">Preferências excluídas e padrão restaurado.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RestaurarPadrao()
        {
            await _service.RestaurarPadraoAsync(ObterUsuarioId());
            return Ok(new { mensagem = "Preferências excluídas com sucesso. As configurações padrão foram restauradas." });
        }

        // Pega o ID do usuário que está dentro do token JWT.
        private int ObterUsuarioId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}