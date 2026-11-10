using GaroAvisa.Api.Filters;
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
    public class CompromissosController : ControllerBase
    {
        private readonly ICompromissoService _service;

        public CompromissosController(ICompromissoService service)
        {
            _service = service;
        }

        /// <summary>Lista os compromissos do usuário logado, do mais próximo para o mais distante.</summary>
        /// <param name="localId">Opcional: mostra só os compromissos desse local.</param>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CompromissoRespostaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar([FromQuery] int? localId)
        {
            return Ok(await _service.ListarAsync(ObterUsuarioId(), localId));
        }

        /// <summary>Busca um compromisso do usuário logado pelo ID.</summary>
        /// <response code="200">Compromisso encontrado.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Compromisso não encontrado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CompromissoRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Buscar(int id)
        {
            var compromisso = await _service.BuscarPorIdAsync(id, ObterUsuarioId());
            return compromisso is null ? NotFound(new { mensagem = "Compromisso não encontrado." }) : Ok(compromisso);
        }

        /// <summary>Mostra a previsão do tempo e os alertas do dia de um compromisso.</summary>
        /// <response code="200">Previsão do compromisso retornada.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Compromisso não encontrado.</response>
        [HttpGet("{id:int}/previsao")]
        [ProducesResponseType(typeof(CompromissoPrevisaoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPrevisao(int id)
        {
            var previsao = await _service.ObterPrevisaoAsync(id, ObterUsuarioId());
            return previsao is null ? NotFound(new { mensagem = "Compromisso não encontrado." }) : Ok(previsao);
        }

        /// <summary>Cadastra um compromisso em um local do usuário logado.</summary>
        /// <remarks>
        /// Envie o cabeçalho Idempotency-Key (ex.: um GUID) para que, se o mesmo pedido for enviado de novo,
        /// a API devolva a mesma resposta sem criar outro compromisso.
        /// </remarks>
        /// <param name="dto">Dados do compromisso.</param>
        /// <param name="idempotencyKey">Chave única do pedido (opcional).</param>
        /// <response code="201">Compromisso criado.</response>
        /// <response code="400">Dados inválidos ou data no passado.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Local não encontrado.</response>
        /// <response code="422">Idempotency-Key já usado em outro pedido.</response>
        [HttpPost]
        [Idempotente]
        [ProducesResponseType(typeof(CompromissoRespostaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Criar(
            [FromBody] SalvarCompromissoDto dto,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
        {
            try
            {
                var criado = await _service.CriarAsync(dto, ObterUsuarioId());
                return criado is null
                    ? NotFound(new { mensagem = "Local não encontrado." })
                    : CreatedAtAction(nameof(Buscar), new { id = criado.Id }, criado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>Atualiza um compromisso do usuário logado.</summary>
        /// <response code="200">Compromisso atualizado.</response>
        /// <response code="400">Dados inválidos ou data no passado.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Compromisso ou local não encontrado.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CompromissoRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarCompromissoDto dto)
        {
            try
            {
                var compromisso = await _service.AtualizarAsync(id, dto, ObterUsuarioId());
                return compromisso is null
                    ? NotFound(new { mensagem = "Compromisso ou local não encontrado." })
                    : Ok(compromisso);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>Exclui um compromisso do usuário logado.</summary>
        /// <response code="204">Compromisso excluído.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Compromisso não encontrado.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _service.ExcluirAsync(id, ObterUsuarioId());
            return excluido ? NoContent() : NotFound(new { mensagem = "Compromisso não encontrado." });
        }

        private int ObterUsuarioId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}