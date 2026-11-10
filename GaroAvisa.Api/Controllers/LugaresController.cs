using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaroAvisa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LugaresController : ControllerBase
    {
        private readonly IClimaService _service;

        public LugaresController(IClimaService service)
        {
            _service = service;
        }

        /// <summary>Busca lugares pelo nome na Open-Meteo, para depois cadastrar como local.</summary>
        /// <remarks>Exemplo: GET /api/Lugares?nome=Tatuapé. Use a latitude, longitude e fuso retornados no POST /api/Locais.</remarks>
        /// <param name="nome">Nome do bairro, cidade ou lugar (mínimo 2 letras).</param>
        /// <response code="200">Lugares encontrados (a lista pode vir vazia).</response>
        /// <response code="400">Nome não informado ou muito curto.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="503">Open-Meteo indisponível no momento.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<LugarDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Buscar([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 2)
            {
                return BadRequest(new { mensagem = "Informe pelo menos 2 letras para buscar." });
            }

            try
            {
                return Ok(await _service.BuscarLugaresAsync(nome));
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { mensagem = "Não foi possível consultar a Open-Meteo agora. Tente novamente." });
            }
        }
    }
}