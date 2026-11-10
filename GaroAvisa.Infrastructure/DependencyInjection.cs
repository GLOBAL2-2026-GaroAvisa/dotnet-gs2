using GaroAvisa.Application.Interfaces;
using GaroAvisa.Application.Services;
using GaroAvisa.Domain.Interfaces;
using GaroAvisa.Infrastructure.Data;
using GaroAvisa.Infrastructure.ExternalServices;
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
            var connectionString = configuration.GetConnectionString("OracleConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(connectionString, oracle =>
                    oracle.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ILocalRepository, LocalRepository>();
            services.AddScoped<IPreferenciaRepository, PreferenciaRepository>();
            services.AddScoped<IRegraAlertaRepository, RegraAlertaRepository>();

            services.AddScoped<IAlertaRepository, AlertaRepository>();
            services.AddScoped<IPrevisaoSalvaRepository, PrevisaoSalvaRepository>();

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ILocalService, LocalService>();
            services.AddScoped<IPreferenciaService, PreferenciaService>();
            services.AddScoped<IRegraAlertaService, RegraAlertaService>();
            services.AddScoped<IClimaService, ClimaService>();

            services.AddScoped<IAlertaService, AlertaService>();
            services.AddScoped<IResumoService, ResumoService>();

            services.AddHttpClient<IOpenMeteoClient, OpenMeteoClient>();

            services.AddSingleton<ITokenService, TokenService>();

            return services;
        }
    }
}