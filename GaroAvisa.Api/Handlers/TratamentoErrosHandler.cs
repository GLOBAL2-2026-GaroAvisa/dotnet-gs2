using System.Data.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace GaroAvisa.Api.Handlers
{
    public class TratamentoErrosHandler : IExceptionHandler
    {
        private readonly ILogger<TratamentoErrosHandler> _logger;

        public TratamentoErrosHandler(ILogger<TratamentoErrosHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int status;
            string mensagem;

            if (EhErroDeBanco(exception))
            {
                status = StatusCodes.Status503ServiceUnavailable;
                mensagem = "O banco de dados está indisponível no momento. Tente novamente em instantes.";
            }
            else if (exception is HttpRequestException || exception is TaskCanceledException)
            {
                status = StatusCodes.Status503ServiceUnavailable;
                mensagem = "Um serviço externo não respondeu. Tente novamente em instantes.";
            }
            else
            {
                status = StatusCodes.Status500InternalServerError;
                mensagem = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
            }

            _logger.LogError(exception, "Erro não tratado em {Metodo} {Rota}. TraceId: {TraceId}",
                httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(new
            {
                mensagem,
                status,
                traceId = httpContext.TraceIdentifier
            }, cancellationToken);

            return true;
        }

        private static bool EhErroDeBanco(Exception? exception)
        {
            while (exception is not null)
            {
                if (exception is DbException)
                {
                    return true;
                }

                exception = exception.InnerException;
            }

            return false;
        }
    }
}