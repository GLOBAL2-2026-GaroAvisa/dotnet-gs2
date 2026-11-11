using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GaroAvisa.Api.Extensions
{
    public static class RespostasErro
    {
        public static IActionResult Validacao(ActionContext context)
        {
            var parametrosDoCorpo = context.ActionDescriptor.Parameters
                .Where(p => p.BindingInfo?.BindingSource == BindingSource.Body)
                .Select(p => p.Name)
                .ToList();

            var erros = context.ModelState
                .Where(e => e.Value!.Errors.Count > 0
                            && e.Key != ""
                            && !e.Key.StartsWith("$")
                            && !parametrosDoCorpo.Contains(e.Key))
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());

            if (erros.Count == 0)
            {
                return new BadRequestObjectResult(new
                {
                    mensagem = "Formato JSON inválido. Confira vírgulas, aspas e chaves do corpo da requisição."
                });
            }

            return new BadRequestObjectResult(new
            {
                mensagem = "Alguns dados estão inválidos. Confira os campos abaixo.",
                erros
            });
        }

        public static void TraduzirMensagensDeLeitura(MvcOptions options)
        {
            var mensagens = options.ModelBindingMessageProvider;
            mensagens.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"O valor '{valor}' não é válido para {campo}.");
            mensagens.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
            mensagens.SetValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
            mensagens.SetUnknownValueIsInvalidAccessor(campo => $"O valor informado para {campo} não é válido.");
            mensagens.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor informado não é válido.");
            mensagens.SetValueMustBeANumberAccessor(campo => $"O campo {campo} deve ser um número.");
            mensagens.SetNonPropertyValueMustBeANumberAccessor(() => "O valor deve ser um número.");
            mensagens.SetValueMustNotBeNullAccessor(valor => $"O valor '{valor}' não pode ser vazio.");
            mensagens.SetMissingBindRequiredValueAccessor(campo => $"Informe o campo {campo}.");
            mensagens.SetMissingKeyOrValueAccessor(() => "Informe um valor.");
            mensagens.SetMissingRequestBodyRequiredValueAccessor(() => "O corpo da requisição é obrigatório.");
        }

        public static async Task StatusSemCorpo(StatusCodeContext contexto)
        {
            var resposta = contexto.HttpContext.Response;
            string? mensagem;

            switch (resposta.StatusCode)
            {
                case StatusCodes.Status401Unauthorized:
                    mensagem = "Você precisa estar logado. Faça login e envie o token.";
                    break;
                case StatusCodes.Status403Forbidden:
                    mensagem = "Você não tem permissão para esta ação. Apenas administradores podem fazer isso.";
                    break;
                case StatusCodes.Status404NotFound:
                    mensagem = "Rota não encontrada. Confira o endereço e o método da requisição.";
                    break;
                case StatusCodes.Status405MethodNotAllowed:
                    mensagem = "Este método não é permitido nesta rota.";
                    break;
                case StatusCodes.Status415UnsupportedMediaType:
                    mensagem = "Tipo de conteúdo não suportado. Envie o corpo em JSON (Content-Type: application/json).";
                    break;
                default:
                    mensagem = null;
                    break;
            }

            if (mensagem is not null)
            {
                await resposta.WriteAsJsonAsync(new { mensagem });
            }
        }
    }
}