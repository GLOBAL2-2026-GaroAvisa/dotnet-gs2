using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace GaroAvisa.Api.Extensions
{
    public static class RateLimitingExtensions
    {
        public const string PoliticaAutenticacao = "autenticacao";

        public static IServiceCollection AddRateLimitingGaroAvisa(this IServiceCollection services, IConfiguration configuration)
        {
            var limiteGeral = configuration.GetValue("RateLimiting:PedidosPorMinuto", 100);
            var limiteAutenticacao = configuration.GetValue("RateLimiting:LoginsPorMinuto", 5);

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: IdentificarCliente(context),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = limiteGeral,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.AddPolicy(PoliticaAutenticacao, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = limiteAutenticacao,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (contexto, cancellationToken) =>
                {
                    if (contexto.Lease.TryGetMetadata(MetadataName.RetryAfter, out var esperar))
                    {
                        contexto.HttpContext.Response.Headers.RetryAfter = ((int)esperar.TotalSeconds).ToString();
                    }

                    await contexto.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        mensagem = "Muitas requisições em pouco tempo. Aguarde um pouco e tente novamente."
                    }, cancellationToken);
                };
            });

            return services;
        }

        private static string IdentificarCliente(HttpContext context)
        {
            var usuarioId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (usuarioId is not null)
            {
                return $"usuario-{usuarioId}";
            }

            return $"ip-{context.Connection.RemoteIpAddress}";
        }
    }
}