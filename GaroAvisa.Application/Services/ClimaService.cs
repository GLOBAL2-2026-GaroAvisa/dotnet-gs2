using GaroAvisa.Application.DTOs;
using GaroAvisa.Application.Interfaces;
using GaroAvisa.Domain.Interfaces;

namespace GaroAvisa.Application.Services
{
    public class ClimaService : IClimaService
    {
        private readonly IOpenMeteoClient _openMeteo;
        private readonly ILocalRepository _localRepository;

        public ClimaService(IOpenMeteoClient openMeteo, ILocalRepository localRepository)
        {
            _openMeteo = openMeteo;
            _localRepository = localRepository;
        }

        public async Task<List<LugarDto>> BuscarLugaresAsync(string nome)
        {
            return await _openMeteo.BuscarLugaresAsync(nome.Trim());
        }

        public async Task<PrevisaoLocalDto?> ObterPrevisaoDoLocalAsync(int localId, int usuarioId, int dias)
        {
            var local = await _localRepository.BuscarPorIdAsync(localId, usuarioId);
            if (local is null)
            {
                return null;
            }

            var previsao = await _openMeteo.ObterPrevisaoAsync(local.Latitude, local.Longitude, local.Fuso, dias);

            return new PrevisaoLocalDto
            {
                LocalId = local.Id,
                Apelido = local.Apelido,
                Nome = local.Nome,
                Dias = previsao
            };
        }
    }
}