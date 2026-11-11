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
    public class LocaisController : ControllerBase
    {
        private readonly ILocalService _service;

        public LocaisController(ILocalService service)
        {
            _service = service;
        }

        /// <summary>Lista os locais do usuário logado.</summary>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<LocalRespostaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.ListarAsync(ObterUsuarioId()));
        }

        /// <summary>Busca um local do usuário logado pelo ID.</summary>
        /// <response code="200">Local encontrado.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Local não encontrado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(LocalRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Buscar(int id)
        {
            var local = await _service.BuscarPorIdAsync(id, ObterUsuarioId());
            return local is null
                ? NotFound(new { mensagem = "Local não encontrado. Confira o ID na lista dos seus locais." })
                : Ok(local);
        }

        /// <summary>Cadastra um novo local para o usuário logado.</summary>
        /// <remarks>Envie o cabeçalho Idempotency-Key para evitar que um pedido repetido crie o local duas vezes.</remarks>
        /// <param name="dto">Dados do local.</param>
        /// <param name="idempotencyKey">Chave única do pedido (opcional).</param>
        /// <response code="201">Local criado.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="409">Apelido já utilizado.</response>
        /// <response code="422">Idempotency-Key já usado em outro pedido.</response>
        [HttpPost]
        [Idempotente]
        [ProducesResponseType(typeof(LocalRespostaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Criar(
            [FromBody] CriarLocalDto dto,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
        {
            try
            {
                var criado = await _service.CriarAsync(dto, ObterUsuarioId());
                return CreatedAtAction(nameof(Buscar), new { id = criado.Id }, criado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>Atualiza apelido, local principal e recebimento de alertas.</summary>
        /// <response code="200">Local atualizado.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Local não encontrado.</response>
        /// <response code="409">Apelido já utilizado.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(LocalRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarLocalDto dto)
        {
            try
            {
                var local = await _service.AtualizarAsync(id, dto, ObterUsuarioId());
                return local is null
                    ? NotFound(new { mensagem = "Local não encontrado. Confira o ID na lista dos seus locais." })
                    : Ok(local);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>Exclui um local do usuário logado.</summary>
        /// <response code="200">Local excluído.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Local não encontrado.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _service.ExcluirAsync(id, ObterUsuarioId());
            return excluido
                ? Ok(new { mensagem = "Local excluído com sucesso." })
                : NotFound(new { mensagem = "Local não encontrado. Confira o ID na lista dos seus locais." });
        }

        private int ObterUsuarioId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}