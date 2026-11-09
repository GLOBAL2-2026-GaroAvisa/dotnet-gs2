using GaroAvisa.Application.Interfaces;
using GaroAvisa.Application.Services;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Infrastructure.Data;
using GaroAvisa.Infrastructure.Repositories;
using GaroAvisa.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GaroAvisa.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Banco de dados Oracle
            var connectionString = configuration.GetConnectionString("OracleConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(connectionString, oracle =>
                    oracle.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

            // 2. Repositórios (acesso ao banco)
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ILocalRepository, LocalRepository>();
            services.AddScoped<IPreferenciaRepository, PreferenciaRepository>();

            // 3. Serviços (regras de negócio)
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ILocalService, LocalService>();
            services.AddScoped<IPreferenciaService, PreferenciaService>();

            // 4. Segurança
            services.AddSingleton<ITokenService, TokenService>();

            return services;
        }
    }
}
