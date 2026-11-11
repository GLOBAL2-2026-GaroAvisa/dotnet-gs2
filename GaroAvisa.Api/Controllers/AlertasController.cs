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
    public class AlertasController : ControllerBase
    {
        private readonly IAlertaService _service;

        public AlertasController(IAlertaService service)
        {
            _service = service;
        }

        /// <summary>Lista os alertas dos locais do usuário logado.</summary>
        /// <param name="somenteNaoLidos">Quando true, mostra só os alertas ainda não lidos.</param>
        /// <response code="200">Lista retornada com sucesso.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<AlertaRespostaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar([FromQuery] bool somenteNaoLidos = false)
        {
            return Ok(await _service.ListarAsync(ObterUsuarioId(), somenteNaoLidos));
        }

        /// <summary>Busca um alerta do usuário logado pelo ID.</summary>
        /// <response code="200">Alerta encontrado.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Alerta não encontrado. Confira o ID na lista dos seus alertas.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AlertaRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Buscar(int id)
        {
            var alerta = await _service.BuscarPorIdAsync(id, ObterUsuarioId());
            return alerta is null ? NotFound(new { mensagem = "Alerta não encontrado. Confira o ID na lista dos seus alertas." }) : Ok(alerta);
        }

        /// <summary>Marca um alerta do usuário logado como lido.</summary>
        /// <response code="200">Alerta marcado como lido.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Alerta não encontrado. Confira o ID na lista dos seus alertas.</response>
        [HttpPut("{id:int}/lido")]
        [ProducesResponseType(typeof(AlertaRespostaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarcarComoLido(int id)
        {
            var alerta = await _service.MarcarComoLidoAsync(id, ObterUsuarioId());
            return alerta is null ? NotFound(new { mensagem = "Alerta não encontrado. Confira o ID na lista dos seus alertas." }) : Ok(alerta);
        }

        /// <summary>Exclui um alerta do usuário logado.</summary>
        /// <response code="200">Alerta excluído.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="404">Alerta não encontrado. Confira o ID na lista dos seus alertas.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _service.ExcluirAsync(id, ObterUsuarioId());
            return excluido
                ? Ok(new { mensagem = "Alerta excluído com sucesso." })
                : NotFound(new { mensagem = "Alerta não encontrado. Confira o ID na lista dos seus alertas." });
        }

        /// <summary>Verifica a previsão de todos os locais e cria os alertas. Somente ADMIN.</summary>
        /// <remarks>
        /// Para cada local com "receber alertas" ligado, busca a previsão de hoje e amanhã na Open-Meteo,
        /// compara com as regras ativas e cria os alertas (sem repetir o mesmo tipo no mesmo dia).
        /// Também compara com a previsão anterior e cria o alerta PREVISAO_MUDOU quando ela muda bastante.
        /// </remarks>
        /// <response code="200">Verificação concluída, com o resumo do que foi feito.</response>
        /// <response code="401">Token ausente ou inválido.</response>
        /// <response code="403">Usuário sem permissão de administrador.</response>
        [HttpPost("verificar")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(VerificacaoResultadoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Verificar()
        {
            return Ok(await _service.VerificarAsync());
        }

        private int ObterUsuarioId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}