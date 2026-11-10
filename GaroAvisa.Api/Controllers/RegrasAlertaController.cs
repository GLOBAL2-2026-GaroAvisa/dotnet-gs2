using GaroAvisa.Api.Filters;
using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GaroAvisa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RegrasAlertaController : ControllerBase
    {
        private readonly IRegraAlertaService _service;

        public RegrasAlertaController(IRegraAlertaService service)
        {
            _service = service;
        }

        /// <summary>Lista as regras que definem quando um alerta é criado.</summary>
        /// <param name="somenteAtivas">Quando true, mostra só as regras ativas.</param>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<RegraAlertaRespostaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar([FromQuery] bool somenteAtivas = false)
        {
            return Ok(await _service.ListarAsync(somenteAtivas));
        }

        /// <summary>Busca uma regra de alerta pelo ID.</summary>
        /// <response code="200">Regra encontrada.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Regra não encontrada.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RegraAlertaRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Buscar(int id)
        {
            var regra = await _service.BuscarPorIdAsync(id);
            return regra is null ? NotFound(new { mensagem = "Regra não encontrada." }) : Ok(regra);
        }

        /// <summary>Cria uma regra de alerta. Somente ADMIN.</summary>
        /// <remarks>
        /// Tipos: CHUVA_FORTE (mm), VENTO_FORTE (km/h), CALOR (°C máxima), FRIO (°C mínima) e TEMPESTADE (código do tempo, ex.: 95).
        /// Gravidades: BAIXA, MEDIA e ALTA.
        /// </remarks>
        /// <param name="dto">Dados da regra.</param>
        /// <param name="idempotencyKey">Chave única do pedido (opcional).</param>
        /// <response code="201">Regra criada.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="403">Usuário sem permissão de administrador.</response>
        /// <response code="409">Já existe regra com esse tipo e gravidade.</response>
        /// <response code="422">Idempotency-Key já usado em outro pedido.</response>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Idempotente]
        [ProducesResponseType(typeof(RegraAlertaRespostaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Criar(
            [FromBody] SalvarRegraAlertaDto dto,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
        {
            try
            {
                var criada = await _service.CriarAsync(dto);
                return CreatedAtAction(nameof(Buscar), new { id = criada.Id }, criada);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>Atualiza uma regra de alerta. Somente ADMIN.</summary>
        /// <response code="200">Regra atualizada.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="403">Usuário sem permissão de administrador.</response>
        /// <response code="404">Regra não encontrada.</response>
        /// <response code="409">Já existe regra com esse tipo e gravidade.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(RegraAlertaRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarRegraAlertaDto dto)
        {
            try
            {
                var regra = await _service.AtualizarAsync(id, dto);
                return regra is null ? NotFound(new { mensagem = "Regra não encontrada." }) : Ok(regra);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>Exclui uma regra de alerta. Somente ADMIN.</summary>
        /// <response code="204">Regra excluída.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="403">Usuário sem permissão de administrador.</response>
        /// <response code="404">Regra não encontrada.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluida = await _service.ExcluirAsync(id);
            return excluida ? NoContent() : NotFound(new { mensagem = "Regra não encontrada." });
        }
    }
}