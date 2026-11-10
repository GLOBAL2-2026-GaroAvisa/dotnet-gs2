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
    public class ResumoController : ControllerBase
    {
        private readonly IResumoService _service;

        public ResumoController(IResumoService service)
        {
            _service = service;
        }

        /// <summary>Resumo do dia do usuário logado: previsão de hoje e alertas de hoje de cada local.</summary>
        /// <remarks>Também informa se o resumo diário está ativo e o horário escolhido nas preferências.</remarks>
        /// <response code="200">Resumo retornado.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResumoDiaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Obter()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _service.ObterResumoAsync(usuarioId));
        }
    }
}