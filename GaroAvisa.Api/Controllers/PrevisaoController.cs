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
    public class PrevisaoController : ControllerBase
    {
        private readonly IClimaService _service;

        public PrevisaoController(IClimaService service)
        {
            _service = service;
        }

        /// <summary>Mostra a previsão do tempo de um local do usuário logado.</summary>
        /// <remarks>Exemplo: GET /api/Previsao/1?dias=3. Os dados vêm da Open-Meteo.</remarks>
        /// <param name="localId">ID do local cadastrado.</param>
        /// <param name="dias">Quantos dias de previsão (de 1 a 7). Padrão: 3.</param>
        /// <response code="200">Previsão retornada.</response>
        /// <response code="400">Quantidade de dias inválida.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Local não encontrado. Confira o ID na lista dos seus locais.</response>
        /// <response code="503">Open-Meteo indisponível no momento.</response>
        [HttpGet("{localId:int}")]
        [ProducesResponseType(typeof(PrevisaoLocalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Obter(int localId, [FromQuery] int dias = 3)
        {
            if (dias < 1 || dias > 7)
            {
                return BadRequest(new { mensagem = "Informe de 1 a 7 dias." });
            }

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var previsao = await _service.ObterPrevisaoDoLocalAsync(localId, usuarioId, dias);
                return previsao is null ? NotFound(new { mensagem = "Local não encontrado. Confira o ID na lista dos seus locais." }) : Ok(previsao);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { mensagem = "O serviço de previsão do tempo está indisponível no momento. Tente novamente em alguns minutos." });
            }
        }
    }
}