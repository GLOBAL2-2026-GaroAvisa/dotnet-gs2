using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using GaroAvisa.Domain.Entities;
using GaroAvisa.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GaroAvisa.Api.Filters
{
    public class IdempotenteAttribute : TypeFilterAttribute
    {
        public IdempotenteAttribute() : base(typeof(IdempotenciaFilter))
        {
        }
    }

    public class IdempotenciaFilter : IAsyncActionFilter
    {
        public const string NomeCabecalho = "Idempotency-Key";

        private static readonly TimeSpan Validade = TimeSpan.FromHours(24);

        private static readonly JsonSerializerOptions OpcoesJson = new(JsonSerializerDefaults.Web)
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly IIdempotenciaRepository _repository;

        public IdempotenciaFilter(IIdempotenciaRepository repository)
        {
            _repository = repository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var chave = context.HttpContext.Request.Headers[NomeCabecalho].ToString();

            if (string.IsNullOrWhiteSpace(chave))
            {
                await next();
                return;
            }

            if (chave.Length > 100)
            {
                context.Result = new BadRequestObjectResult(new { mensagem = "O Idempotency-Key pode ter até 100 caracteres." });
                return;
            }

            var usuario = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var usuarioId = usuario is null ? 0 : int.Parse(usuario);
            var rota = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            var hash = CalcularHash(context.ActionArguments);

            var existente = await _repository.BuscarAsync(chave, usuarioId, DateTime.UtcNow - Validade);

            if (existente is not null)
            {
                if (existente.Rota != rota || existente.HashRequisicao != hash)
                {
                    context.Result = new UnprocessableEntityObjectResult(new
                    {
                        mensagem = "Esse Idempotency-Key já foi usado em outro pedido. Gere uma chave nova."
                    });
                    return;
                }

                // Mesmo pedido de novo: devolve a resposta guardada, sem executar outra vez.
                context.HttpContext.Response.Headers["Idempotent-Replay"] = "true";
                context.Result = new ContentResult
                {
                    StatusCode = existente.StatusCode,
                    Content = existente.RespostaJson,
                    ContentType = "application/json; charset=utf-8"
                };
                return;
            }

            var executado = await next();

            if (executado.Result is ObjectResult resultado)
            {
                var status = resultado.StatusCode ?? 200;
                if (status >= 200 && status < 300)
                {
                    await _repository.SalvarAsync(new RegistroIdempotencia
                    {
                        Chave = chave,
                        UsuarioId = usuarioId,
                        Rota = rota,
                        HashRequisicao = hash,
                        StatusCode = status,
                        RespostaJson = JsonSerializer.Serialize(resultado.Value, OpcoesJson)
                    });
                }
            }
        }

        private static string CalcularHash(IDictionary<string, object?> argumentos)
        {
            var json = JsonSerializer.Serialize(argumentos);
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(bytes);
        }
    }
}